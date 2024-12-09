using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BridgeScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private SpriteRenderer part1Renderer;
    [SerializeField] private SpriteRenderer part2Renderer;
    [SerializeField] private float duration;

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
        rb.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        float counter = 0;
        //Get current color
        Color spriteColor1 = part1Renderer.material.color;
        Color spriteColor2 = part2Renderer.material.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            //Fade from 1 to 0
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            Debug.Log(alpha);

            //Change alpha only
            part1Renderer.color = new Color(spriteColor1.r, spriteColor1.g, spriteColor1.b, alpha);
            part2Renderer.color = new Color(spriteColor2.r, spriteColor2.g, spriteColor2.b, alpha);
            //Wait for a frame
            yield return null;
        }
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
