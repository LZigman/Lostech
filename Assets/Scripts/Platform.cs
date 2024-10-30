using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class Platform : MonoBehaviour
{
    private PlatformEffector2D platformEffector2D;
    private Player player;
    private bool isColliding;

    private void Start()
    {
        platformEffector2D = GetComponent<PlatformEffector2D>();
        player = FindObjectOfType<Player>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isColliding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isColliding = false;
        }
    }

    private void Update()
    {
        if (player.isJumping)
        {
            platformEffector2D.rotationalOffset = 0;
            StartCoroutine(ResetOffset(2f));
        }

        if (player.isDropDown && isColliding)
        {
            platformEffector2D.rotationalOffset = 180;
            StartCoroutine(ResetOffset(1.5f));
        }
    }

    private IEnumerator ResetOffset(float delay)
    {
        yield return new WaitForSeconds(delay);
        platformEffector2D.rotationalOffset = 0;
    }
}
