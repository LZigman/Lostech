using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
	[SerializeField] private float bulletSpeed;
	private Rigidbody2D rb;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = bulletSpeed * transform.right;									// giving velocity to bullet
	}
}