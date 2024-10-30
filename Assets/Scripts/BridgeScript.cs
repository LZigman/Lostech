using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BridgeScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }
    private void OnCollisionEnter2D (Collision2D other)
    {
        if (CompareLayers(other.gameObject, playerLayer) == true)
        {
            StartCoroutine(BridgeFall());
        }
    }

    private IEnumerator BridgeFall ()
    {
        yield return new WaitForSeconds(0.5f);
        rb.bodyType = RigidbodyType2D.Dynamic;
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
