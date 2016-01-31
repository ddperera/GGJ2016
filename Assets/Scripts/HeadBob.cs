using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

	private float timer = 0.0f; 
	public float bobbingSpeed = 0.18f; 
	public float bobbingAmount = 0.2f; 
	private Vector3 vectChange;
	private Movement movementScr;
	private bool canLerp;
	private bool actuallySprinting;
	//private AudioSource myAudSrc;
	public AudioClip[] footsteps;
	Camera camera;

	void Start(){
		movementScr = GetComponentInParent<Movement>();
		canLerp = true;
		//myAudSrc = GetComponent<AudioSource>();
		camera = GetComponent<Camera>();
	}

	void Update () {
		bobbingSpeed = movementScr.crouching ? .1f : .18f;

		float waveslice = 0.0f; 
		float horizontal = Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.D) ? 0f : Input.GetAxis ("Horizontal"); 
		float vertical = Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.S) ? 0f : Input.GetAxis ("Vertical"); 

		if (movementScr.midCrouch || movementScr.crouching)
		{
			horizontal = vertical = 0f;
		}

		if (Mathf.Abs (horizontal) == 0f && Mathf.Abs (vertical) == 0f)
		{ 
			timer = 0.0f; 
		} 
		else
		{ 
			waveslice = Mathf.Sin (timer) - 1; 
			timer = timer + bobbingSpeed; 
			if (timer > Mathf.PI * 2f)
			{ 
				timer = timer - (Mathf.PI * 2f); 
			} 
		} 
		if (waveslice != 0f)
		{ 
			float translateChange = waveslice * bobbingAmount; 
			float totalAxes = Mathf.Abs (horizontal) + Mathf.Abs (vertical); 
			totalAxes = !movementScr.crouching ? Mathf.Clamp (totalAxes, 0.0f, 1.0f) : 0f;
			translateChange = totalAxes * translateChange + movementScr.controller.height/4.0f; 
			vectChange = new Vector3(0f,translateChange,0f);
			transform.localPosition = vectChange; 
		} else
		{ 
			//transform.localPosition.y = midpoint; 
		}

		if (Mathf.Abs (waveslice + 2) < .01)
		{
			//myAudSrc.clip = footsteps[Random.Range (0, footsteps.Length)];
			//myAudSrc.PlayOneShot(footsteps[Random.Range (0, footsteps.Length)]);
		}
	}

}
