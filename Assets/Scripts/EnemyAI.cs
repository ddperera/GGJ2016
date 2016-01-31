using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

	public float walkSpeed = 2f;
	public float chaseSpeed = 3f;
	public float chaseTimeout;
	public float[] patrolWaitTimes;
	public Transform[] waypoints;
	public GameManager gameManager;

	private EnemyVision enemyVision;
	private NavMeshAgent nav;
	private GameObject player;
	private float endChaseTimer;
	private float patrolTimer;
	private int waypointIndex;

	void Awake()
	{
		enemyVision = GetComponent<EnemyVision> ();
		nav = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update()
	{
		if (enemyVision.canSeePlayer || enemyVision.lastSightingLoc != enemyVision.resetLoc)
		{
			Chase ();
		} 
		else
		{
			Patrol ();
		}

	}

	void Chase()
	{
		Vector3 vecToPlayer = player.transform.position - transform.position;

		nav.speed = chaseSpeed;

		nav.destination = enemyVision.lastSightingLoc;

		if (nav.remainingDistance <= nav.stoppingDistance)
		{
			endChaseTimer += Time.deltaTime;

			if (endChaseTimer > chaseTimeout)
			{
				enemyVision.lastSightingLoc = enemyVision.resetLoc;
				endChaseTimer = 0f;
			}
		}

		if (Vector3.SqrMagnitude (vecToPlayer) < 2 && enemyVision.canSeePlayer)
		{
			player.GetComponent<Movement>().TakeDamage ();
		}
	}

	void Patrol()
	{
		nav.speed = walkSpeed;
		if (nav.destination == enemyVision.resetLoc || nav.remainingDistance < nav.stoppingDistance)
		{
			patrolTimer += Time.deltaTime;

			if (patrolTimer >= patrolWaitTimes [waypointIndex])
			{
				if (waypointIndex == waypoints.Length - 1)
				{
					waypointIndex = 0;
				} 
				else
				{
					waypointIndex++;
				}

				patrolTimer = 0;
			} 
		}
		else
		{
			patrolTimer = 0;
		}

		nav.destination = waypoints [waypointIndex].position;
	}
}
