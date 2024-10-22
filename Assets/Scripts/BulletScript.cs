using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
	[SerializeField] private float bulletSpeed, bulletDamage;
	[SerializeField] private LayerMask enemyLayer, groundLayer, spitterLayer, locustLayer;
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
			StartCoroutine(other.gameObject.GetComponent<Spitter>().Damage(bulletDamage));
		}
		else if (CompareLayers(other.gameObject, locustLayer) == true)
		{
			StartCoroutine(other.gameObject.GetComponent<Locust>().Die());
		}
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
