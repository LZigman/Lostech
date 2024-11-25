using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileScript : MonoBehaviour
{
    public float projectileSpeed;
    public Vector2 direction;

    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(projectileSpeed * direction, ForceMode2D.Impulse);
    }
}
