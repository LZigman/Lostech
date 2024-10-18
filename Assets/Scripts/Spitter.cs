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
    [SerializeField] private Animator animator;
    
    // private variables
    private Rigidbody2D rb;
    private GameObject player;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
        
        StartCoroutine(Patrol());
	}
	private IEnumerator Patrol ()
    {
        Vector2 moveTo;
        animator.SetBool("isMoving", true);
        while (true)
        {
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
                animator.SetBool("isMoving", false);
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
                Debug.Log("Player Detected!");
                return true;
			}
		}
        return false;
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
            yield return new WaitForSeconds (Time.fixedDeltaTime);
        }
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
