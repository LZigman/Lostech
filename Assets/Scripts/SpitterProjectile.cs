using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
	// serializable variables
	[SerializeField] private float playerHitAnimationLength = 0.3f, groundHitAnimationLength = 0.35f;
	[SerializeField] private LayerMask playerLayer, groundLayer;
	// public variables
	[HideInInspector] public float distanceFromPlayer;
	[HideInInspector] public Vector2 dir;
	
	// private variables
	private Rigidbody2D rb;
	private Animator animator;
	private float force;
	private int travelAnimationId = Animator.StringToHash("Travel");
	private int playerHitAnimationId = Animator.StringToHash("Burst 2");
	private int groundHitAnimationId = Animator.StringToHash("burst 1");
	private void Start()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		force = distanceFromPlayer;
		rb.AddForce(force * dir, ForceMode2D.Impulse);
		AnimationStateChanger.Instance.ChangeAnimationState(travelAnimationId, animator);
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (CompareLayers(other.gameObject, playerLayer) == true)
		{
			rb.bodyType = RigidbodyType2D.Static;
			Debug.Log("Player hit!");
			StartCoroutine(PlayerHitAnimation());
		}
		if (CompareLayers (other.gameObject, groundLayer) == true)
		{
			rb.bodyType = RigidbodyType2D.Static;
			Debug.Log("Ground Hit!");
			StartCoroutine(GroundHitAnimation());
		}
	}
	private IEnumerator PlayerHitAnimation ()
	{
		AnimationStateChanger.Instance.ChangeAnimationState(playerHitAnimationId, animator);
		yield return new WaitForSeconds(playerHitAnimationLength);
		Destroy(gameObject);
	}
	private IEnumerator GroundHitAnimation()
	{
		AnimationStateChanger.Instance.ChangeAnimationState (groundHitAnimationId, animator);
		yield return new WaitForSeconds(groundHitAnimationLength);
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
	
}
