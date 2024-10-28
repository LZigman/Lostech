using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class KillZone : MonoBehaviour
{
    private BoxCollider2D killArea;
    private Player player;
    
    private void Start()
    {
        killArea = GetComponent<BoxCollider2D>();
        player = FindObjectOfType<Player>();
        killArea.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.isDead = true;
        }
    }
}
