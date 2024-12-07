using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileScript : MonoBehaviour
{
    public float projectileSpeed;
    public float projectileDamage;
    public Vector2 direction;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.AddForce(projectileSpeed * transform.right, ForceMode2D.Impulse);
        rb.velocity = projectileSpeed * direction;
        Debug.LogError("dir of bullet: " + direction);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareLayers(other.gameObject, playerLayer) == true)
        {
            other.gameObject.GetComponent<Player>().DamagePlayer(projectileDamage);
        }
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
