using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image[] hpBars;

    private void Start()
    {
        IncreaseHealthBarToFull();
    }

    public void ReduceHealthBar(int currentHealth)
    {
        hpBars[currentHealth].gameObject.SetActive(false);
    }
    
    public void IncreaseHealthBar(int currentHealth)
    {
        hpBars[currentHealth].gameObject.SetActive(true);
    }

    public void IncreaseHealthBarToFull()
    {
        foreach (var hpBar in hpBars)
        {
            hpBar.gameObject.SetActive(true);
        }
    }
}
