using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
	[SerializeField] private float bulletSpeed;
	[SerializeField] private LayerMask enemyLayer;
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
			Debug.Log("Enemy Hit!");
		}
	}

	bool CompareLayers (GameObject objectWithLayer, LayerMask layerMask)
	{
		if ((layerMask.value & (1 << objectWithLayer.gameObject.layer)) != 0)
		{
			return true;
		}
		return false;
	}
}
