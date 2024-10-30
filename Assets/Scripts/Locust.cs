using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    public Transform playerTransform;
    [SerializeField] private float attackRadius;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackAnimationDelay;
    [SerializeField] private float attackAnimationLength;
    [SerializeField] private float deathAnimationLength;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private int flyingAnimationId = Animator.StringToHash("Fly");
    private int attackAnimationId = Animator.StringToHash("attack");
    private int deathAnimationId = Animator.StringToHash("Death");
    private Coroutine activeCoroutine;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
        activeCoroutine = StartCoroutine(Fly());
	}
	private IEnumerator Fly ()
    {
        Vector2 tempDirection = rb.position;
        AnimationStateChanger.Instance.ChangeAnimationState(flyingAnimationId, animator);
        while (true)
        {
            if (Vector2.Distance(rb.position, (Vector2)playerTransform.position) < attackRadius)
            {
                activeCoroutine = StartCoroutine(Attack());
                yield break;
            }
            if (Vector2.Distance(tempDirection, rb.position) < 0.1f)
            {
                tempDirection = RandomDirTowards(playerTransform.position);
                if (Vector2.Distance(tempDirection, playerTransform.position) < 2f)
                {
                    tempDirection = playerTransform.position;
                }
            }
            rb.position = Vector2.MoveTowards(rb.position, tempDirection, movementSpeed * Time.fixedDeltaTime);
            if (tempDirection.x > rb.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
				transform.localScale = new Vector3(1, 1, 1);
			}
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    private IEnumerator Attack ()
    {
        AnimationStateChanger.Instance.ChangeAnimationState(attackAnimationId, animator);
        yield return new WaitForSeconds(attackAnimationDelay);
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, attackRadius);
        for (int i = 0; i < detectedColliders.Length; i++)
        {
            if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
            {
                detectedColliders[i].gameObject.GetComponent<Player>().DamagePlayer(damage);
                activeCoroutine = StartCoroutine(Die());
                yield break;
            }
        }
        activeCoroutine = StartCoroutine(Die());
    }
    public IEnumerator Die ()
    {
        StopCoroutine(activeCoroutine);
        AnimationStateChanger.Instance.ChangeAnimationState(deathAnimationId, animator);
        Debug.Log("Animation started!\nlen: " + deathAnimationLength);
        Debug.Log("Time scale: " + Time.timeScale);
        //yield return new WaitForSeconds(0.6f);
        Invoke(nameof(DestroySelf), 0.6f);
        Debug.Log("Destroying self!");
        //Destroy(gameObject);
        yield return null;
    }
    private void DestroySelf ()
    {
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
	// dir je position playera tj pozicije prema kojoj se randomly trebamo pomaknuti
    private Vector2 RandomDirTowards (Vector2 dir)
    {
        Vector2 moveTowards;
        if (dir.y <= transform.position.y)
        {
            // random vektor po y izmeðu 0 i +beskonaèno                        <- po y
            // po x treba random izmeðu trenutne pozicije i dir.x
            moveTowards.y = Random.Range(dir.y, dir.y + 5f);
        }
        else
        {
            // random vektor izmeðu player.position.y i 2 * rb.pos.y - dir.y    <- po y
            // random.position
            moveTowards.y = Random.Range(dir.y, 2 * rb.position.y - dir.y);
        }
        moveTowards.x = Random.Range(Mathf.Min(rb.position.x, dir.x), Mathf.Max(rb.position.x, dir.x));
        return moveTowards;
        
    }
}
