using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	// public variables
	[Header("Stats")]
	[SerializeField] private float movementSpeed;
	[SerializeField] private float maxHealth = 100f;
	[SerializeField] private float damage = 10f;
	[SerializeField] private bool isReturnToStartingPos = true;
	[SerializeField] private LayerMask playerLayer;
	[Space(5)]
	[Header("Radii")]
	[SerializeField] private float detectionRadius;
	[SerializeField] private float attackRadius;
	[Space(5)]
	[Header("Animations")]
	[Tooltip("Amount of time between start of animation and when enemy should hit player.")]
	[SerializeField] private Animator animator;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private float attackDelay = 0.3f;
	[SerializeField] private float attackAnimationLength = 0.7f;
	[SerializeField] private float wakeUpAnimationLength = 0.4f;
	[SerializeField] private float sleepAnimationLength = 0.43f;
	[SerializeField] private float hitAnimationLength = 0.4f;
	[SerializeField] private float dieAnimationLength = 0.8f;

	// private variables
	private Rigidbody2D rb;
	private Vector2 startingPos;
	private GameObject player;
	private float currentHealth;
	private bool isDying;
	private bool isHurt;
	public enum States
	{
		idle,
		moveToAttack,
		attack
	}
	public States currentState;
	

	private void Start()
	{
		currentHealth = maxHealth;
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
		Debug.Log("Wake up!");
		currentState = States.moveToAttack;
		animator.SetBool("isPlayerDetected", true);
		AudioManager.Instance.PlaySFX("ghoul wake up");
		yield return new WaitForSeconds(wakeUpAnimationLength);
		StartCoroutine (MoveToAttack());
	}
	private IEnumerator Sleep ()
	{
		Debug.Log("Sleep!");
		currentState = States.idle;
		AudioManager.Instance.PlaySFX("ghoul sleep");
		yield return new WaitForSeconds (sleepAnimationLength);
		animator.SetBool("isPlayerDetected", false);
		player = null;
		StartCoroutine(DetectPlayer());
	}
	private IEnumerator MoveToAttack ()
	{
		Debug.Log("Moving to attack!");
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
			if (Mathf.Abs(player.transform.position.y - rb.position.y) > 2f)
			{
				yield return new WaitForSeconds(Time.fixedDeltaTime);
			}
			if (isHurt == false)
			{
				if (IsPlayerInAttackRadius() == true)
				{
					StartCoroutine(Attack());
					yield break;
				}
				if (IsPlayerInDetectionRadius() == false)
				{
					if (isReturnToStartingPos == true)
					{
						StartCoroutine(MoveToStartingPos());
						yield break;
					}
					else
					{
						animator.SetBool("isMoving", false);
						StartCoroutine(Sleep());
						yield break;
					}
				}
				rb.position = Vector2.MoveTowards (rb.position, player.transform.position, movementSpeed * Time.fixedDeltaTime);
			}
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	private IEnumerator MoveToStartingPos ()
	{
		Debug.Log("Moving Back!");
		currentState = States.moveToAttack;
		animator.SetBool("isMoving", true);
		if (startingPos.x < rb.position.x)
		{
			spriteRenderer.flipX = true;
		}
        else
        {
            spriteRenderer.flipX = false;
        }
		while (true)
		{
			if (isHurt == false)
			{
				if (Vector2.Distance(startingPos, rb.position) < 0.25f)
				{
					spriteRenderer.flipX = false;
					animator.SetBool("isMoving", false);
					StartCoroutine(Sleep());
					yield break;
				}
				if (IsPlayerInDetectionRadius() == true)
				{
					StartCoroutine(MoveToAttack());
					yield break;
				}
				rb.position = Vector2.MoveTowards(rb.position, startingPos, movementSpeed * Time.fixedDeltaTime);
			}
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
    }
	private IEnumerator Attack ()
	{
		Debug.Log("Starting attack!");
		currentState = States.attack;
		animator.SetBool("isAttacking", true);
		while (true)
		{
			if (isHurt == false)
			{
				if (IsPlayerInAttackRadius() == false)
				{
					StartCoroutine(MoveToAttack());
					animator.SetBool("isAttacking", false);
					yield break;
				}
				yield return new WaitForSeconds(attackDelay);								// wait for animation to hit player
				Debug.Log("Attack!");
				AudioManager.Instance.PlaySFX("ghoul attack");
				PerformAttack();
			}
			yield return new WaitForSeconds(attackAnimationLength - attackDelay);       // wait for end of animation
		}
	}

	private void PerformAttack ()
	{
		// raycast toward player, if hit player deal damage
		Vector2 dir = (Vector2)player.transform.position - rb.position;
		RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, attackRadius, playerLayer);
		if (hit.collider != null)
		{
			Player playerScript = hit.collider.gameObject.GetComponent<Player>();
			playerScript.DamagePlayer(damage);
		}
	}
	public void Damage (float damage)
	{
		StartCoroutine(PerformDamage(damage));
	}
	private IEnumerator PerformDamage (float damage)
	{
		if (isDying == true)
		{
			yield break;
		}
		isHurt = true;
		currentHealth -= damage;
		AudioManager.Instance.PlaySFX("enemy hit");
		Debug.Log("health: " + currentHealth);
		if (currentHealth <= 0f)
		{
			StartCoroutine(Die());
			yield break;
		}
		animator.SetBool("isHurt", true);
		yield return new WaitForSecondsRealtime(hitAnimationLength);
		Debug.LogError("Damage animation performed!");
        animator.SetBool("isHurt", false);
        isHurt = false;
		yield break;
		//animator.ResetTrigger("hit");
	}
	private IEnumerator Die ()
	{
		isDying = true;
		AudioManager.Instance.PlaySFX("enemy death");
		animator.SetTrigger("die");
		yield return new WaitForSeconds(dieAnimationLength);
		Destroy(gameObject);
	}
	// helper functions
	private float GetCurrentAnimationLength()
	{
		return animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length;
	}

    private bool IsPlayerInDetectionRadius ()
	{
		if (Vector2.Distance ((Vector2)player.transform.position, rb.position) < detectionRadius + 0.25f)
		{
			return true;
		}
		return false;
	}
	private bool IsPlayerInAttackRadius ()
	{
		if (Vector2.Distance (player.transform.position, rb.position) < attackRadius)
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
