using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    public Transform playerTransform;
    public Vector2 direction;
    [SerializeField] private float attackRadius;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackAnimationDelay;
    [SerializeField] private float attackAnimationLength;
    [SerializeField] private float deathAnimationLength;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float randomDirectionRadius = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private int randomDirectionCount;

	private void Start()
	{
		randomDirectionCount = 0;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        direction = (Vector2)playerTransform.position;                                              // kad napravim Summoner skriptu stavit ovo u nju
        if (direction.x >= rb.position.x)
        {
            direction += new Vector2(-1, -0.5f);
        }
        else
        {
			direction += new Vector2(1, -0.5f);
		}
        StartCoroutine(Fly());
	}
	private IEnumerator Fly ()
    {
        Vector2 tempDirection = rb.position;
        animator.SetBool("isFlying", true);
        while (true)
        {
            if (Vector2.Distance(rb.position, playerTransform.position) < attackRadius)
            {
                animator.SetBool("isFlying", false);
                StartCoroutine(Attack());
                yield break;
            }
            if (tempDirection.x > rb.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
				transform.localScale = new Vector3(1, 1, 1);
			}
            if (Vector2.Distance(tempDirection, rb.position) < 0.1f)
            {
                tempDirection = RandomDirTowardsPlayer();
                randomDirectionCount++;
                if (randomDirectionCount == 3)
                {
                    tempDirection = playerTransform.position;
                }
            }
            rb.position = Vector2.MoveTowards(rb.position, tempDirection, movementSpeed * Time.fixedDeltaTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    private IEnumerator Attack ()
    {
        Debug.Log("Attack Triggered!");
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(attackAnimationDelay);
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, attackRadius);
        for (int i = 0; i < detectedColliders.Length; i++)
        {
            if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
            {
                detectedColliders[i].gameObject.GetComponent<Player>().DamagePlayer(damage);
				Debug.Log("Player attacked!");
				StartCoroutine(Die());
                yield break;
            }
        }
    }
    public IEnumerator Die ()
    {
		animator.SetBool("isFlying", false);
		animator.SetBool("isAttacking", false);
        //animator.SetBool("isDeath", true);
        animator.SetTrigger("Die");
        rb.bodyType = RigidbodyType2D.Static;
		yield return new WaitForSeconds(deathAnimationLength);
        Debug.Log("DIE!!!");
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
	// odabere random poziciju iznat playera u radijusu od randomDirectionRadius
    private Vector2 RandomDirTowardsPlayer ()
    {
        Vector2 randomDirection;
        randomDirection.x = playerTransform.position.x + Random.Range(-randomDirectionRadius, randomDirectionRadius);
        randomDirection.y = playerTransform.position.y + Random.Range(1f, randomDirectionRadius);
        return randomDirection;
    }
}
