using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float moveSpeed;
	public float sensitivity;
	public float crouchTime = 0.75f;

	private Camera camera;
	private CharacterController controller;
	private Vector3 vel;
	private float targetHeight;
	private RaycastHit hitInfo;
	private Ray toFloor;
	private Ray testShot;
	private bool crouching = false;
	private bool midCrouch = false;
	private float cameraRotY = 0f;



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
	}
	
	// Update is called once per frame
	void Update () 
	{
		camera.transform.localRotation = Quaternion.Euler (camera.transform.localRotation.eulerAngles.x, 0, 0);
		vel.x = Input.GetAxisRaw ("Horizontal");
		vel.y = 0; 
		vel.z = Input.GetAxisRaw ("Vertical");
		vel.Normalize ();
		vel = transform.TransformDirection (vel);
		controller.Move (moveSpeed * vel * Time.deltaTime);

		toFloor.origin = transform.position;
		toFloor.direction = -1 * transform.up;
		Physics.Raycast (toFloor, out hitInfo);

		if (Mathf.Abs (hitInfo.distance - targetHeight) > .05) 
		{
			transform.position = new Vector3 (transform.position.x, hitInfo.point.y + targetHeight, transform.position.z);
		}

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
		float finalYPos = camera.transform.position.y - (controller.height / 2.0f);
		float startYPos = camera.transform.position.y;
		float curYPos = startYPos;

		controller.height /= 2.0f;
		transform.position = new Vector3 (transform.position.x, transform.position.y - (controller.height / 2.0f), transform.position.z);
		targetHeight /= 2.0f;

		moveSpeed /= 2.0f;
		for (float t = 0; t <= crouchTime; t += Time.deltaTime)
		{
			curYPos = Mathf.Lerp (startYPos, finalYPos, t / crouchTime);
			camera.transform.position = new Vector3 (camera.transform.position.x, curYPos, camera.transform.position.z);
			yield return null;
		}

		crouching = true;

		midCrouch = false;
	}

	IEnumerator Uncrouch()
	{
		
		controller.height *= 2.0f;
		float finalYPos = camera.transform.position.y + (controller.height / 2.0f);
		float startYPos = camera.transform.position.y;
		float curYPos = startYPos;

		transform.position = new Vector3 (transform.position.x, transform.position.y + (controller.height / 2.0f), transform.position.z);
		targetHeight *= 2.0f;



		for (float t = 0; t <= crouchTime; t += Time.deltaTime)
		{
			curYPos = Mathf.Lerp (startYPos, finalYPos, t / crouchTime);
			camera.transform.position = new Vector3 (camera.transform.position.x, curYPos, camera.transform.position.z);
			yield return null;
		}

		crouching = false;
		moveSpeed *= 2.0f;
		midCrouch = false;
	}
}
