using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Summoner : MonoBehaviour
{
    [SerializeField] private GameObject locustPrefab;
	[SerializeField] private Transform patrolLimiterLeft, patrolLimiterRight;
	[SerializeField] private Transform spawnPos;
	[SerializeField] private bool patrolStartLeft = true;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float swarmSize, summoningRate;
	[SerializeField] private float summonAnimationDelay;
	[SerializeField] private Animator animator;
	[SerializeField] private LayerMask playerLayer;
	[SerializeField] private float detectionRadius;
	[SerializeField] private float maxHealth = 100f;

    private Rigidbody2D rb;
	private Transform playerTransform;
	private float currentHealth;
	private int walkAnimationId = Animator.StringToHash("Move");
	private int summonAnimationId = Animator.StringToHash("Summon");
	private int hitAnimationId = Animator.StringToHash("Hit");
	private int deathAnimationId = Animator.StringToHash("Death");
	private int idleAnimationId = Animator.StringToHash("Idle");
	private bool isHit, isSummoning;

	private void Awake()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
		rb = GetComponent<Rigidbody2D>();
		StartCoroutine(Patrol());
		currentHealth = maxHealth;
	}

	private IEnumerator Patrol ()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(walkAnimationId, animator);
		Vector2 moveTo;
		while (true)
		{
			// check if player detected
			if (IsPlayerDetected() == true)
			{
				animator.SetBool("isMoving", false);
				StartCoroutine(PlayerDetected());
				yield break;
			}
			// if hit -> play hit animation
			if (isHit == true)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(hitAnimationId, animator);
				yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
				AnimationStateChanger.Instance.ChangeAnimationState(walkAnimationId, animator);
				isHit = false;
			}
			// check if arrived at left patrol limiter
			if (Mathf.Abs(rb.position.x - patrolLimiterLeft.position.x) < 0.1f)
			{
				patrolStartLeft = false;
			}
			// check if arrived at right patrol limiter
			else if (Mathf.Abs(rb.position.x - patrolLimiterRight.position.x) < 0.1f)
			{
				patrolStartLeft = true;
			}
			// check if should move left
			if (patrolStartLeft == true)
			{
				FlipLeft(true);
				moveTo = (Vector2)patrolLimiterLeft.position;
				moveTo.y = rb.position.y;
			}
			// check if should move right
			else
			{
				FlipLeft(false);
				moveTo = (Vector2)patrolLimiterRight.position;
				moveTo.y = rb.position.y;
			}
			// moving the rigidbody
			rb.position = Vector2.MoveTowards(rb.position, moveTo, movementSpeed * Time.fixedDeltaTime);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	private IEnumerator PlayerDetected ()
	{
		while (true)
		{
			// check if player still in detection radius
			if (IsPlayerDetected () == false)
			{
				StartCoroutine(Patrol());
				playerTransform = null;
				yield break;
			}
			// summon
			AnimationStateChanger.Instance.ChangeAnimationState(summonAnimationId, animator);
			isSummoning = true;
			for (int i = 0; i < swarmSize; i++)
			{
				yield return new WaitForSeconds(summonAnimationDelay);
				GameObject temp = Instantiate(locustPrefab, spawnPos.position, Quaternion.identity);
				temp.GetComponent<Locust>().playerTransform = playerTransform;
				yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0).Length - summonAnimationDelay);
			}
			isSummoning = false;
			AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
			yield return new WaitForSeconds(summoningRate);
		}
	}
	public void Damage (float damage)
	{
		// invounrable if summoning
		if (isSummoning == false)
		{
			currentHealth -= damage;
			if (currentHealth <= 0f)
			{
				StartCoroutine(Death());
				return;
			}
			StartCoroutine (DamageAnimation());
			Debug.Log("Damage registered!");
			isHit = true;
		}
	}
	private IEnumerator DamageAnimation ()
	{
		AnimationStateChanger.Instance.ChangeAnimationState (hitAnimationId, animator);
		yield return new WaitForSeconds (animator.GetCurrentAnimatorClipInfo(layerIndex: 0).Length);
		AnimationStateChanger.Instance.ChangeAnimationState (idleAnimationId, animator);
	}
	private IEnumerator Death()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(deathAnimationId, animator);
		yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0).Length);
		Destroy(gameObject);
	}
	// helper functions
	private bool CompareLayers(GameObject objectWithLayer, LayerMask layerMask)
	{
		if ((layerMask.value & (1 << objectWithLayer.gameObject.layer)) != 0)
		{
			return true;
		}
		return false;
	}
	private void FlipLeft (bool toggle)
	{
		if (toggle == true)
		{
			transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}
	private bool IsPlayerDetected ()
	{
		Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
		for (int i = 0; i < detectedColliders.Length; i++)
		{
			if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
			{
				playerTransform = detectedColliders[i].transform;
				if (playerTransform.position.x < rb.position.x)
				{
					FlipLeft(true);
				}
				else
				{
					FlipLeft(false);
				}
				return true;
			}
		}
		return false;
	}
}
