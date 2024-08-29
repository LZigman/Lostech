using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadiusScript : MonoBehaviour
{
	public Enemy enemy;
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player") == true)
		{
			Debug.Log("Player entered Attack Radius!");
			StopCoroutine(enemy.activeCoroutine);
			enemy.activeCoroutine = StartCoroutine(enemy.Attack());
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player") == true)
		{
			Debug.Log("Player exited Attack Radius!");
			StopCoroutine(enemy.activeCoroutine);
		}
	}
}
