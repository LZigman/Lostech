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
	[Tooltip("Amount of time between start of animation and when enemy should hit player.")]
	[SerializeField] private float attackDelay = 0.3f;
	[SerializeField] private float attackAnimationLength = 0.7f;
	[SerializeField] private float wakeUpAnimationLength = 0.4f;
	[SerializeField] private float sleepAnimationLength = 0.43f;
	[Space(2)]
	[SerializeField] private float movementSpeed;
	[SerializeField] private Animator animator;
	[SerializeField] private SpriteRenderer spriteRenderer;

	// private variables
	private Rigidbody2D rb;
	private Vector2 startingPos;
	private GameObject player;
	public enum States
	{
		idle,
		moveToAttack,
		attack
	}
	public States currentState;
	

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		startingPos = rb.position;
		StartCoroutine(DetectPlayer());
	}
	private IEnumerator DetectPlayer ()
	{
		currentState = States.idle;
		while (true)
		{
			Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
			for (int i = 0; i < detectedColliders.Length; i++)
			{
				if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
				{
					player = detectedColliders[i].gameObject;
					StartCoroutine(WakeUp());
					yield break;
				}
			}
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	private IEnumerator WakeUp ()
	{
		currentState = States.moveToAttack;
		animator.SetBool("isPlayerDetected", true);
		yield return new WaitForSeconds(wakeUpAnimationLength);
		StartCoroutine (MoveToAttack());
	}
	private IEnumerator Sleep ()
	{
		currentState = States.idle;
		yield return new WaitForSeconds (sleepAnimationLength);
		animator.SetBool("isPlayerDetected", false);
		StartCoroutine(DetectPlayer());
	}
	private IEnumerator MoveToAttack ()
	{
		currentState = States.moveToAttack;
		animator.SetBool("isMoving", true);
		if (player.transform.position.x < rb.position.x)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;
		}
		while (true)
		{
			if (IsPlayerInAttackRadius() == true)
			{
				StartCoroutine(Attack());
				yield break;
			}
			if (IsPlayerInDetectionRadius() == false)
			{
				animator.SetBool("isMoving", false);
				StartCoroutine(Sleep());
				yield break;
			}
			rb.position = Vector2.MoveTowards (rb.position, player.transform.position, movementSpeed * Time.fixedDeltaTime);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	private IEnumerator Attack ()
	{
		currentState = States.attack;
		animator.SetBool("isAttacking", true);
		while (true)
		{
			if (IsPlayerInAttackRadius() == false)
			{
				StartCoroutine(MoveToAttack());
				animator.SetBool("isAttacking", false);
				yield break;
			}
			yield return new WaitForSeconds(attackDelay);								// wait for animation to hit player
			Debug.Log("Attack!");
			yield return new WaitForSeconds(attackAnimationLength - attackDelay);       // wait for end of animation
		}
	}

	// helper functions
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
