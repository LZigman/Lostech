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
	private bool isHit;

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
			Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
			for (int i = 0; i < detectedColliders.Length; i++)
			{
				if (IsPlayerDetected() == true)
				{
					animator.SetBool("isMoving", false);
					playerTransform = detectedColliders[i].transform;
					StartCoroutine(PlayerDetected());
					yield break;
				}
			}
			if (isHit == true)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(hitAnimationId, animator);
				Debug.Log("Hit animation len: " + animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
				yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
				AnimationStateChanger.Instance.ChangeAnimationState(walkAnimationId, animator);
				isHit = false;
			}
			if (Mathf.Abs(rb.position.x - patrolLimiterLeft.position.x) < 0.1f)
			{
				patrolStartLeft = false;
			}
			else if (Mathf.Abs(rb.position.x - patrolLimiterRight.position.x) < 0.1f)
			{
				patrolStartLeft = true;
			}
			if (patrolStartLeft == true)
			{
				FlipLeft(true);
				moveTo = (Vector2)patrolLimiterLeft.position;
				moveTo.y = rb.position.y;
			}
			else
			{
				FlipLeft(false);
				moveTo = (Vector2)patrolLimiterRight.position;
				moveTo.y = rb.position.y;
			}
			rb.position = Vector2.MoveTowards(rb.position, moveTo, movementSpeed * Time.fixedDeltaTime);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	private IEnumerator PlayerDetected ()
	{
		/*
		 * transition into idle
		 * transition into summoning
		 * transition back into idle
		 * if player exited detection radius transition back into patrol
		 * */
		while (true)
		{
			AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
			if (IsPlayerDetected () == false)
			{
				StartCoroutine(Patrol());
				playerTransform = null;
				yield break;
			}
			if (isHit == true)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(hitAnimationId, animator);
				Debug.Log("Hit animation len: " + animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
				yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
				AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
				Debug.Log("Animation Performed!");
				isHit = false;
			}
			// summon
			AnimationStateChanger.Instance.ChangeAnimationState(summonAnimationId, animator);
			for (int i = 0; i < swarmSize; i++)
			{
				yield return new WaitForSeconds(summonAnimationDelay);
				GameObject temp = Instantiate(locustPrefab, spawnPos.position, Quaternion.identity);
				temp.GetComponent<Locust>().playerTransform = playerTransform;
				yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0).Length);
			}
			AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
			yield return new WaitForSeconds(summoningRate);
		}
	}
	public void Damage (float damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 0f)
		{
			StartCoroutine(Death());
			return;
		}
		Debug.Log("Damage registered!");
		isHit = true;
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
