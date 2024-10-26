using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Spitter : MonoBehaviour
{
    // public variables
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform patrolLimiterLeft, patrolLimiterRight;
    [SerializeField] private bool patrolStartLeft = true;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackAnimationDelay = 0.33f;
    [SerializeField] private float attackAnimationLength = 0.8f;
    [SerializeField] private float damageAnimationLength = 0.8f;
    [SerializeField] private float deathAnimationLength = 0.8f;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float maxHealth = 1f;
    
    // private variables
    private Rigidbody2D rb;
    private GameObject player;
    private float timeAtLastAttack;
    private float currentHealth;
    private int walkAnimationId = Animator.StringToHash("Walk");
    private int hitAnimationId = Animator.StringToHash("hit");
    private int deathAnimationId = Animator.StringToHash("death");
    private int attackAnimationId = Animator.StringToHash("attack");
    private int idleAnimationId = Animator.StringToHash("Idle");
    private bool isHit;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
        timeAtLastAttack = 0f;
        currentHealth = maxHealth;
        StartCoroutine(Patrol());
	}
	private IEnumerator Patrol ()
    {
        Vector2 moveTo;
        AnimationStateChanger.Instance.ChangeAnimationState(walkAnimationId, animator);
        while (true)
        {
            if (isHit == true)
            {
                AnimationStateChanger.Instance.ChangeAnimationState(hitAnimationId, animator);
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
                transform.localScale = new Vector3(-1f, 1f, 1f);
                moveTo = (Vector2)patrolLimiterLeft.position;
                moveTo.y = rb.position.y;
            }
            else
            {
				transform.localScale = new Vector3(1f, 1f, 1f);
				moveTo = (Vector2)patrolLimiterRight.position;
				moveTo.y = rb.position.y;
			}
            rb.position = Vector2.MoveTowards (rb.position, moveTo, movementSpeed * Time.fixedDeltaTime);
            if (DetectPlayer() == true)
            {
                AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
                StartCoroutine(ShootingState());
                yield break;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    
    private bool DetectPlayer ()
    {
		Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
		for (int i = 0; i < detectedColliders.Length; i++)
		{
			if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
			{
                player = detectedColliders[i].gameObject;
                if (player.transform.position.x < rb.position.x)
                {
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
                else
                {
					transform.localScale = new Vector3(1f, 1f, 1f);
				}
                return true;
			}
		}
        return false;
	}
    public void Damage (float damage)
    {

        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            StartCoroutine(Death());
            return;
        }
        isHit = true;
    }
    public IEnumerator Death ()
    {
        Debug.Log("death!");
        AnimationStateChanger.Instance.ChangeAnimationState(deathAnimationId, animator);
		yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
        Destroy(gameObject);
    }
    private IEnumerator ShootingState ()
    {
        while (true)
        {
            if (DetectPlayer () == false)
            {
                Debug.Log("Player escaped!");
                StartCoroutine(Patrol());
                yield break;
            }
			if (isHit == true)
			{
				AnimationStateChanger.Instance.ChangeAnimationState(hitAnimationId, animator);
				yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length);
				AnimationStateChanger.Instance.ChangeAnimationState(walkAnimationId, animator);
				isHit = false;
			}
			if (Time.time > timeAtLastAttack + attackRate)
            {
                StartCoroutine(Shoot());
                timeAtLastAttack = Time.time;
            }
            yield return new WaitForSeconds (Time.fixedDeltaTime);
        }
    }

    private IEnumerator Shoot ()
    {
        AnimationStateChanger.Instance.ChangeAnimationState(attackAnimationId, animator);
        yield return new WaitForSeconds(attackAnimationDelay);
        var temp = Instantiate (projectile, projectileSpawnPoint.position, Quaternion.identity);
        var temp2 = temp.GetComponent<SpitterProjectile>();
        if (transform.localScale.x == 1)
        {
            temp2.dir = Vector2.one;
            temp2.distanceFromPlayer = Mathf.Abs(player.transform.position.x - projectileSpawnPoint.position.x);
        }
        else
        {
			temp.GetComponent<SpitterProjectile>().dir = Vector2.left + Vector2.up;
		}
        yield return new WaitForSeconds(attackAnimationLength - attackAnimationDelay);
        AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
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
}