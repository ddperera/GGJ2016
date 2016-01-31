using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour {

	public float fovAngle = 100f;
	public bool canSeePlayer;
	public Vector3 lastSightingLoc;

	private GameObject player;
	private SphereCollider visionVolume;
	private NavMeshAgent nav;
	public Vector3 resetLoc;

	void Awake()
	{
		resetLoc = new Vector3 (999f, 999f, 999f);

		nav = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		visionVolume = GetComponent<SphereCollider> ();

		lastSightingLoc = resetLoc;
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player) 
		{
			canSeePlayer = false;

			Vector3 dir = other.transform.position - transform.position;
			float angleToPlayer = Vector3.Angle (dir, transform.forward);

			if (angleToPlayer < fovAngle * 0.5f) 
			{
				RaycastHit hitInfo;

				if (Physics.Raycast (transform.position, dir, out hitInfo, visionVolume.radius)) 
				{
					if (hitInfo.collider.gameObject == player) 
					{
						canSeePlayer = true;
						lastSightingLoc = player.transform.position;
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player)
		{
			canSeePlayer = false;
		}
	}
}
