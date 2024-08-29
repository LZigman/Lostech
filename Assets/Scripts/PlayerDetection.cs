using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public Enemy enemy;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") == true)
		{
			Debug.Log("Player entered detection Radius!");
			enemy.playerTransform = other.transform;
			enemy.activeCoroutine = StartCoroutine(enemy.WalkTowardsPlayer());
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") == true)
		{
			Debug.Log("Player exited detection Radius!");
			StopCoroutine(enemy.activeCoroutine);
			enemy.activeCoroutine = StartCoroutine(enemy.ReturnToStartingPos());
		}
	}
}
