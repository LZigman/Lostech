using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
	[SerializeField] private float bulletSpeed, bulletDamage, destroyDelay = 1f;
	[SerializeField] private LayerMask enemyLayer, groundLayer, spitterLayer, locustLayer, summonerLayer;
	private Rigidbody2D rb;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		
		// giving velocity to bullet
		rb.velocity = bulletSpeed * transform.right;
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (CompareLayers(other.gameObject, enemyLayer) == true)
		{
			StartCoroutine (other.gameObject.GetComponent<Enemy>().Damage(bulletDamage));
		}
		else if (CompareLayers(other.gameObject, spitterLayer) == true)
		{
			other.gameObject.GetComponent<Spitter>().Damage(bulletDamage);
		}
		else if (CompareLayers(other.gameObject, locustLayer) == true)
		{
			StartCoroutine(other.gameObject.GetComponent<Locust>().Die());
			Debug.Log("Locust Hit!");
		}
		else if (CompareLayers(other.gameObject, summonerLayer) == true)
		{
			other.gameObject.GetComponent<Summoner>().Damage(bulletDamage);
		}
		Destroy(gameObject);

	}
	private IEnumerator DestroyDelay ()
	{
		yield return new WaitForSeconds(destroyDelay);
		Destroy(gameObject);
	}
	// helper function
	private bool CompareLayers(GameObject objectWithLayer, LayerMask layerMask)
	{
		if ((layerMask.value & (1 << objectWithLayer.gameObject.layer)) != 0)
		{
			return true;
		}
		return false;
	}
}
