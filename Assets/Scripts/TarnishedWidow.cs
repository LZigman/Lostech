using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TarnishedWidow : MonoBehaviour
{
    public enum States
    {
        idle,
        walking,
        attack,
        damage,
        death
    }

    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxHealth;
    // heavy attack radius
    [SerializeField] private float detectionRadius, attackRadius;
    [SerializeField] private float heavyAttackDamage, spitAttackDamage;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float attackWaitTime = 0.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform heavyAttackPointA, heavyAttackPointB;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    // private variables
    private Rigidbody2D rb;
    private GameObject player;
    // heavy attack radius hardcoded because its always the same
    private float heavyAttackRadius = 4f;
    private States currentState;
    private float currentHealth;
    private float spitAttackDelay = 0.33f, heavyAttackDelay = 0.38f;
    private int spitAttackAnimationId = Animator.StringToHash("Attack 2");
    private int heavyAttackAnimationId = Animator.StringToHash("Attack 1");
    private int walkingAnimationId = Animator.StringToHash("Walking");
    private int idleAnimationId = Animator.StringToHash("Idle");
    private int damageAnimationId = Animator.StringToHash("Damage");
    private int deathAnimationId = Animator.StringToHash("Death");
    private bool isHurt;

    private void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentState = States.idle;
        isHurt = false;
    }
    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.transform.position.x < rb.position.x)
            {
                FlipLeft(true);
            }
            else
            {
                FlipLeft(false);
            }
        }
        if (currentState == States.idle && isHurt == false)
        {
            DetectPlayer();
        }
        if (currentState == States.walking && isHurt == false)
        {
            MoveTowardsPlayer();
        }
    }
    private void DetectPlayer ()
    {
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
        for (int i = 0; i < detectedColliders.Length; i++)
        {
            if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
            {
                player = detectedColliders[i].gameObject;
                currentState = States.walking;
                return;
            }
        }
    }

    private void MoveTowardsPlayer ()
    {
        if (IsPlayerInAttackRadius() == true)
        {
            StartCoroutine(AttackPattern());
            currentState = States.attack;
            return;
        }
        Vector2 dirTowardsPlayer = (Vector2)player.transform.position - rb.position;
        rb.position += dirTowardsPlayer * movementSpeed * Time.fixedDeltaTime;
    }
    private IEnumerator AttackPattern ()
    {
        while (IsPlayerInAttackRadius() == true && isHurt == false)
        {
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
                        temp.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                    else
                    {
                        temp.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    temp.GetComponent<Rigidbody2D>().AddForce(projectileSpeed * transform.right, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(GetCurrentAnimationLength() - spitAttackDelay);
                    //AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
                }
            }
            AnimationStateChanger.Instance.ChangeAnimationState(idleAnimationId, animator);
            yield return new WaitForSeconds(attackWaitTime);
        }
        currentState = States.walking;
    }

    public IEnumerator Damage (float damage)
    {
        isHurt = true;
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
        isHurt = false;
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
            Debug.Log("Player in attack radius!");
            return true;
        }
        return false;
    }
    private bool isPlayerInHeavyAttackRadius ()
    {
        if (Vector2.Distance(player.transform.position, rb.position) <= heavyAttackRadius)
        {
            Debug.Log("Player in heavy attack radius!");
            return true;
        }
        return false;
    }
}
