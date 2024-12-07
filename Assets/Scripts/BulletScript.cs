using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
	[SerializeField] private float bulletSpeed, bulletDamage, destroyDelay = 1f;
	[SerializeField] private LayerMask enemyLayer, groundLayer, spitterLayer, locustLayer, summonerLayer, tarnishedWidowLayer;
	private Rigidbody2D rb;
	private Player player;
	private void Start()
	{
		player = FindObjectOfType<Player>();
		rb = GetComponent<Rigidbody2D>();
		
		// giving velocity to bullet
		rb.velocity = bulletSpeed * transform.right;
		rb.velocity += new Vector2((player.GetComponent<Rigidbody2D>().velocity.x), (player.GetComponent<Rigidbody2D>().velocity.y))  ;
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (CompareLayers(other.gameObject, enemyLayer) == true)
		{
			other.gameObject.GetComponent<Enemy>().Damage(bulletDamage);
            Destroy(gameObject);
        }
		else if (CompareLayers(other.gameObject, spitterLayer) == true)
		{
			other.gameObject.GetComponent<Spitter>().Damage(bulletDamage);
            Destroy(gameObject);
        }
		else if (CompareLayers(other.gameObject, locustLayer) == true)
		{
			other.gameObject.GetComponent<Locust>().Die();
            Destroy(gameObject);
            Debug.Log("Locust Hit!");
		}
		else if (CompareLayers(other.gameObject, summonerLayer) == true)
		{
			other.gameObject.GetComponent<Summoner>().Damage(bulletDamage);
            Destroy(gameObject);
        }
		else if (CompareLayers(other.gameObject, tarnishedWidowLayer) == true)
		{
			StartCoroutine(other.gameObject.GetComponent<TarnishedWidow>().Damage(bulletDamage));
			Destroy(gameObject);
		}
		else if (CompareLayers(other.gameObject, groundLayer) == true && !other.CompareTag("Platform"))
		{
			Destroy(gameObject);
		}
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
