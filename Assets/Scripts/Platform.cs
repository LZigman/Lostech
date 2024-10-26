using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class Platform : MonoBehaviour
{
    private PlatformEffector2D platformEffector2D;
    private Player player;

    private void Start()
    {
        platformEffector2D = GetComponent<PlatformEffector2D>();
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (player.isJumping)
        {
            platformEffector2D.rotationalOffset = 0;
            StartCoroutine(ResetOffset());
        }

        if (player.isDropDown)
        {
            platformEffector2D.rotationalOffset = 180;
            StartCoroutine(ResetOffset());
        }
    }

    private IEnumerator ResetOffset()
    {
        yield return new WaitForSeconds(0.7f);
        platformEffector2D.rotationalOffset = 0;
    }
}
