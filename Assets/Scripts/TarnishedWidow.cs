using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarnishedWidow : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float detectionRadius, attackRadius, heavyAttackRadius;
    [SerializeField] private float heavyAttackDamage, spitAttackDamage;
    [SerializeField] private float attackWaitTime;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform heavyAttackPointA, heavyAttackPointB;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    // private variables
    private Rigidbody2D rb;
    private GameObject player;

    private float currentHealth;
    private float spitAttackDelay, heavyAttackDelay;
    private int spitAttackAnimationId = Animator.StringToHash("Attack 2");
    private int heavyAttackAnimationId = Animator.StringToHash("Attack 1");
    private int walkingAnimationId = Animator.StringToHash("Walking");
    private int idleAnimationId = Animator.StringToHash("Idle");
    private int damageAnimationId = Animator.StringToHash("Damage");
    private int deathAnimationId = Animator.StringToHash("Death");

    private void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }
    private IEnumerator DetectPlayer ()
    {
        while (true)
        {
            Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
            for (int i = 0; i < detectedColliders.Length; i++)
            {
                if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
                {
                    player = detectedColliders[i].gameObject;
                    StartCoroutine(MoveTowardsPlayer());
                    yield break;
                }
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    private IEnumerator MoveTowardsPlayer ()
    {
        while (true)
        {
            
            if (IsPlayerInAttackRadius() == true)
            {
                StartCoroutine(AttackPattern());
                yield break;
            }
            Vector2 dirTowardsPlayer = (Vector2)player.transform.position - rb.position;
            if (dirTowardsPlayer.x < 0f)
            {
                FlipLeft(true);
            }
            else
            {
                FlipLeft(false);
            }
            rb.position += dirTowardsPlayer * movementSpeed * Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    private IEnumerator AttackPattern ()
    {
        if (player.transform.position.x < rb.position.x)
        {
            FlipLeft(true);
        }
        else
        {
            FlipLeft(false);
        }
        if (isPlayerInHeavyAttackRadius () == true)
        {
            // heavy attack
            AnimationStateChanger.Instance.ChangeAnimationState(heavyAttackAnimationId, animator);
            yield return new WaitForSeconds(heavyAttackDelay);
            Collider2D[] colliders = Physics2D.OverlapAreaAll(heavyAttackPointA.position, heavyAttackPointB.position);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (CompareLayers(colliders[i].gameObject, playerLayer) == true)
                {
                    player.GetComponent<Player>().DamagePlayer(heavyAttackDamage);
                    break;
                }
            }
            yield return new WaitForSeconds(GetCurrentAnimationLength() - heavyAttackDelay);
        }
        else
        {
            // spit attack x2
            for (int i = 0; i < 2; i++)
            {
                AnimationStateChanger.Instance.ChangeAnimationState(spitAttackAnimationId, animator);
                yield return new WaitForSeconds(spitAttackDelay);
                // spit
                GameObject temp = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                if (transform.localScale.x == -1f)
                {
                    temp.GetComponent<BossProjectileScript>().direction = Vector2.left;
                }
                else
                {
                    temp.GetComponent<BossProjectileScript>().direction = Vector2.right;
                }
                yield return new WaitForSeconds(GetCurrentAnimationLength() - spitAttackDelay);
            }
        }
        AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
        yield return new WaitForSeconds(attackWaitTime);
    }

    private IEnumerator Damage (float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            StartCoroutine(Death());
            yield break;
        }
        else
        {
            AnimationStateChanger.Instance.ChangeAnimationState(damageAnimationId, animator);
            yield return new WaitForSeconds(GetCurrentAnimationLength());
        }
    }
    private IEnumerator Death ()
    {
        AnimationStateChanger.Instance.ChangeAnimationState(deathAnimationId, animator);
        yield return new WaitForSeconds(GetCurrentAnimationLength());
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
    private float GetCurrentAnimationLength ()
    {
        return animator.GetCurrentAnimatorClipInfo(layerIndex: 0)[0].clip.length;
    }
    private bool IsPlayerInAttackRadius ()
    {
        if (Vector2.Distance(player.transform.position, rb.position) <= attackRadius)
        {
            return true;
        }
        return false;
    }
    private bool isPlayerInHeavyAttackRadius ()
    {
        if (Vector2.Distance(player.transform.position, rb.position) <= heavyAttackRadius)
        {
            return true;
        }
        return false;
    }
}
