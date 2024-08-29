using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public bool isPlayerDetected;
	public Transform playerTransform;
	public Coroutine activeCoroutine; 

	[SerializeField] private float walkingSpeed, detectionRadius, attackRaidus, attackRate;

	private Rigidbody2D rb;
	private Vector2 startingPos;
	
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		startingPos = rb.position;
	}

	private void WakeUp ()
	{
		// trigger wake animation
		// start moving toward player
		// if player within smaller circle (detect through physics2D.drawCircle) attack
		// if player exits detection radius return to starting position and sleep
		Debug.Log("Wake Up!");
	}

	public IEnumerator WalkTowardsPlayer ()
	{
		while (true)
		{
			Debug.Log("Walking towards Player!");
			rb.position = walkingSpeed * Vector2.MoveTowards (rb.position, playerTransform.position, Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	public IEnumerator ReturnToStartingPos ()
	{
		while (true)
		{
			Debug.Log("Returning to starting pos!");
			rb.position = walkingSpeed * Vector2.MoveTowards(rb.position, startingPos, Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	public IEnumerator Attack ()
	{
		while (true)
		{
			Debug.Log("Attack!");
			yield return new WaitForSeconds(attackRate);
		}
	}
}
