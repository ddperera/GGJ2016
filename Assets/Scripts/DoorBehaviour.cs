using UnityEngine;
using System.Collections;

public class DoorBehaviour : MonoBehaviour {

	bool shut = true;
	public float openTime = 1f;
	public GameObject pivotTransform;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnPlayerClicked()
	{
		if (shut)
		{
			shut = false;
			StartCoroutine ("OpenDoor");
		}
	}

	IEnumerator OpenDoor()
	{
		Vector3 pivotPoint = pivotTransform.transform.position;
		for (float t = 0; t < openTime; t += Time.deltaTime)
		{
			transform.RotateAround (pivotPoint, Vector3.up, -120f / openTime * Time.deltaTime);
			yield return null;
		}
	}
}
