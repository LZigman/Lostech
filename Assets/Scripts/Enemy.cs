using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	// public variables
	public Coroutine activeCoroutine;
	public LayerMask playerLayer;
	[Header("Radii")]
	[SerializeField] private float detectionRadius;
	[SerializeField] private float attackRadius;
	[Header("Rates")]
	[SerializeField] private float attackRate;
	[Space(2)]
	[SerializeField] private float movementSpeed;
	[SerializeField] private Animator animator;

	// private variables
	private Rigidbody2D rb;
	private Vector2 startingPos, posOfPlayer;
	private GameObject player;
	private int isPlayerInAttackRadiusId, isPlayerDetectedId;
	// length of wake animation clip
	private float wakeClipTime;
	public enum States
	{
		idle,
		moveToAttack,
		attack
	}
	public States currentState;
	private float timeAtLastAttack;
	private float timeAtPlayerDetected;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		startingPos = rb.position;
		currentState = States.idle;
		timeAtLastAttack = Time.time;
		isPlayerDetectedId = Animator.StringToHash("isPlayerDetected");
		isPlayerInAttackRadiusId = Animator.StringToHash("isPlayerInAttackRadius");
	}

	private void FixedUpdate()
	{
		if (currentState == States.idle)
		{
			// detect player
			DetectPlayer();
		}
		if (currentState == States.moveToAttack)
		{
			// moving towards player
			MoveTowardsPlayer();
			
		}
		if (currentState == States.attack)
		{
			// attack player
			if (timeAtLastAttack + attackRate <= Time.time)
			{
				Attack();
				timeAtLastAttack = Time.time;
			}
		}
	}
	
	private void DetectPlayer ()
	{
		Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
		for (int i = 0; i < detectedColliders.Length; i++)
		{
			if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
			{
				currentState = States.moveToAttack;
				player = detectedColliders[i].gameObject;
				animator.SetBool ("isPlayerDetected", true);
				timeAtPlayerDetected = Time.time;
			}
		}
	}
	
	private void MoveTowardsPlayer ()
	{
		// move towards player
		Vector2 moveTo = (Vector2)player.transform.position;
		moveTo.y = rb.position.y;
		if (timeAtPlayerDetected + 0.4f <= Time.time)
		{
			rb.position = Vector2.MoveTowards(rb.position, moveTo, movementSpeed * Time.fixedDeltaTime);
		}
		// check if player in attack radius
		if (IsPlayerInAttackRadius () == true)
		{
			currentState = States.attack;
			animator.SetBool("isPlayerInAttackRadius", true);
		}
		// check if player exited detection radius
		if (IsPlayerInDetectionRadius() == false)
		{
			currentState = States.idle;
			animator.SetBool("isPlayerDetected", false);
			player = null;
		}
	}
	
	private void Attack ()
	{
		Debug.Log("Attack!");
		// check if player exited attack radius
		if (IsPlayerInAttackRadius () == false)
		{
			currentState = States.moveToAttack;
			animator.SetBool ("isPlayerInAttackRadius", false);
		}
	}
	private bool IsPlayerInDetectionRadius ()
	{
		if (Vector2.Distance ((Vector2)player.transform.position, rb.position) <= detectionRadius)
		{
			return true;
		}
		return false;
	}
	private bool IsPlayerInAttackRadius ()
	{
		if (Vector2.Distance (player.transform.position, rb.position) <= attackRadius)
		{
			return true;
		}
		return false;
	}
	private bool CompareLayers(GameObject objectWithLayer, LayerMask layerMask)
	{
		if ((layerMask.value & (1 << objectWithLayer.gameObject.layer)) != 0)
		{
			return true;
		}
		return false;
	}

}
