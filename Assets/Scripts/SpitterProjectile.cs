using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    public Vector2 dir;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float detectionRadius;
	[SerializeField] private LayerMask playerLayer, groundLayer;
	private Rigidbody2D rb;
	private void Awake()
	{
		dir = Vector2.zero;
		dir = CalculateDir(dir);
		StartCoroutine(MoveProjectile());
	}
	private IEnumerator MoveProjectile ()
	{
		while (true)
		{
			rb.position += movementSpeed * Time.fixedDeltaTime * dir;
			DetectPlayer();
			dir = CalculateDir(dir);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	private void DetectPlayer ()
	{
		Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(rb.position, detectionRadius);
		for (int i = 0; i < detectedColliders.Length; i++)
		{
			if (CompareLayers(detectedColliders[i].gameObject, playerLayer) == true)
			{
				dir = (Vector2)detectedColliders[i].transform.position - rb.position;
				return;
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (CompareLayers(other.gameObject, playerLayer) == true)
		{
			Debug.Log("Player hit!");
			Destroy(gameObject);
		}
		if (CompareLayers (other.gameObject, groundLayer) == true)
		{
			Debug.Log("Ground Hit!");
			Destroy(gameObject);
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
	// calculates dir for next physic update
	private Vector2 CalculateDir (Vector2 dir)
	{
		Vector2 dir2 = dir;
		dir2.y = -0.25f * (dir.x - 2) * (dir.x - 2) + 1;
		return dir2;
	}
}
