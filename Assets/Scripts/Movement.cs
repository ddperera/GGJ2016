using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float moveSpeed;
	public float sensitivity;
	public float crouchTime = 0.75f;
	public int hitpoints = 1;
	public float invincibilityTime = 3f;

	private Camera camera;
	public CharacterController controller;
	public Vector3 vel;
	private float targetHeight;
	private RaycastHit hitInfo;
	private Ray toFloor;
	private Ray testShot;
	public bool crouching = false;
	public bool midCrouch = false;
	private float cameraRotY = 0f;
	private float crouchCameraY;
	private float standingCameraY;
	public enum state {WALKING, LADDER, AUTO, GUILOCK};
	public state curState;
	private Vector3 temp;
	private Quaternion tempQ;
	private bool invincible = false;
	public GameManager gameMgr;


	// Use this for initialization
	void Start () 
	{
		vel = new Vector3 ();
		controller = GetComponent<CharacterController> ();
		camera = GetComponentInChildren<Camera> ();
		toFloor = new Ray (transform.position, -1 * transform.up);
		Physics.Raycast (toFloor, out hitInfo);
		targetHeight = hitInfo.distance;

		testShot = new Ray (new Vector3 (-1, 1, -3), Vector3.right);

		standingCameraY = camera.transform.position.y;
		crouchCameraY = camera.transform.position.y - controller.height / 2.0f;

		curState = state.WALKING;
	}
	
	// Update is called once per frame
	void Update () 
	{
		camera.transform.localRotation = Quaternion.Euler (camera.transform.localRotation.eulerAngles.x, 0, 0);
		if (curState == state.WALKING)
		{
			vel.x = Input.GetAxisRaw ("Horizontal");
			vel.y = 0; 
			vel.z = Input.GetAxisRaw ("Vertical");
			vel = transform.TransformDirection (vel);
			vel.Normalize ();
			toFloor.origin = transform.position;
			toFloor.direction = -1 * transform.up;
			Physics.Raycast (toFloor, out hitInfo);
			if (Mathf.Abs (hitInfo.distance - targetHeight) > .05) 
			{
				transform.position = new Vector3 (transform.position.x, hitInfo.point.y + targetHeight, transform.position.z);
			}
		} 
		else if (curState == state.LADDER)
		{
			vel.x = vel.z = 0;
			vel.y = Input.GetAxisRaw ("Vertical");
			vel.Normalize ();
			vel = crouching ? vel * moveSpeed / 100.0f : vel * moveSpeed / 50.0f;
		}
		else if(curState == state.AUTO)
		{
			vel.x = vel.y = vel.z = 0;
		}
		
		controller.Move (moveSpeed * vel * Time.deltaTime);

		transform.Rotate(Vector3.up, Input.GetAxis ("MouseX") * Time.deltaTime * sensitivity);

		Vector3 goalCamRot = camera.transform.localEulerAngles;
		float amountToMoveY = -1 * Input.GetAxis ("MouseY") * sensitivity * Time.deltaTime;
		goalCamRot.x += amountToMoveY;
		if (goalCamRot.x < 265f && goalCamRot.x > 180f) {
			goalCamRot.x = 265f;
		} else if (goalCamRot.x > 80f && goalCamRot.x < 180f) {
			goalCamRot.x = 80f;
		}
		camera.transform.localEulerAngles = goalCamRot;

		if (Input.GetKey (KeyCode.C))
		{
			if (!midCrouch )
			{
				midCrouch = true;
				if (crouching)
				{
					StartCoroutine ("Uncrouch");
				} 
				else
				{
					StartCoroutine ("Crouch");
				}
			}
		}

	}

	IEnumerator Crouch()
	{
		float startYPos = camera.transform.position.y;
		float curYPos = startYPos;

		controller.height /= 2.0f;
		transform.position = new Vector3 (transform.position.x, transform.position.y - (controller.height / 2.0f), transform.position.z);
		targetHeight /= 2.0f;

		moveSpeed /= 2.0f;
		for (float t = 0; t <= crouchTime; t += Time.deltaTime)
		{
			curYPos = Mathf.Lerp (startYPos, crouchCameraY, t / crouchTime);
			camera.transform.position = new Vector3 (camera.transform.position.x, curYPos, camera.transform.position.z);
			yield return null;
		}

		crouching = true;

		midCrouch = false;
	}

	IEnumerator Uncrouch()
	{
		
		controller.height *= 2.0f;
		float startYPos = camera.transform.position.y;
		float curYPos = startYPos;

		transform.position = new Vector3 (transform.position.x, transform.position.y + (controller.height / 2.0f), transform.position.z);
		targetHeight *= 2.0f;



		for (float t = 0; t <= crouchTime; t += Time.deltaTime)
		{
			curYPos = Mathf.Lerp (startYPos, standingCameraY, t / crouchTime);
			camera.transform.position = new Vector3 (camera.transform.position.x, curYPos, camera.transform.position.z);
			yield return null;
		}

		crouching = false;
		moveSpeed *= 2.0f;
		midCrouch = false;
	}

	private void OnTriggerEnter(Collider coll){
		if(coll.gameObject.CompareTag("LadderTop") || coll.gameObject.CompareTag("LadderBottom"))
		{
			if(curState == state.WALKING)
			{
				StartCoroutine(MountLadder(coll, transform));
			}
			else if(curState == state.LADDER)
			{
				StartCoroutine(DismountLadder(coll, transform));
			}
		}
	}

	private void OnTriggerStay(Collider coll)
	{
		if (curState == state.LADDER)
		{
			if (coll.gameObject.CompareTag ("LadderTop"))
			{
				if (Input.GetAxisRaw ("Vertical") > 0)
				{
					StartCoroutine (DismountLadder (coll, transform));
				}
			}
			if (coll.gameObject.CompareTag ("LadderBottom"))
			{
				if (Input.GetAxisRaw ("Vertical") < 0)
				{
					StartCoroutine (DismountLadder (coll, transform));
				}
			}
		}
	}

	private IEnumerator DismountLadder(Collider coll, Transform playerInit){
		curState = state.AUTO;
		Transform dest = coll.gameObject.GetComponentsInChildren<Transform>()[1];
		for (float t=0; t<1f; t+=Time.smoothDeltaTime){
			transform.position = temp = Vector3.Lerp(playerInit.position, dest.position, t);
			transform.rotation = tempQ = Quaternion.Lerp (playerInit.rotation, dest.rotation, t);
			yield return null;
		}
		transform.position = dest.position;
		transform.rotation = dest.rotation;
		curState = state.WALKING;
	}

	private IEnumerator MountLadder(Collider coll, Transform playerInit){
		curState = state.AUTO;
		Transform dest = coll.gameObject.transform;
		for (float t=0; t<1f; t+=Time.smoothDeltaTime){
			transform.position = temp = Vector3.Lerp(playerInit.position, dest.position, t);
			transform.rotation = tempQ = Quaternion.Lerp (playerInit.rotation,dest.rotation, t);
			yield return null;
		}
		transform.position = dest.position;
		transform.rotation = dest.rotation;
		curState = state.LADDER;
	}

	public void TakeDamage()
	{
		if (hitpoints == 0)
		{
			//gameMgr.Reset ();
		} 
		else if(!invincible)
		{
			hitpoints--;
			invincible = true;
			StartCoroutine ("Invincibility");
		}
	}

	IEnumerator Invincibility()
	{
		for (float t = 0; t < invincibilityTime; t += Time.deltaTime)
		{
			yield return null;
		}
		invincible = false;
	}
}
