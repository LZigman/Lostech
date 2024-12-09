using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
	[SerializeField] private TarnishedWidow tarnishedWidow;
	[SerializeField] private GameObject endScreen;
	[SerializeField] private float delay;

	private void Update()
	{
		if (tarnishedWidow.currentHealth <= 0)
		{
			StartCoroutine(ShowEndScreen());
		}
	}

	private IEnumerator ShowEndScreen()
	{
		yield return new WaitForSeconds(delay);
		endScreen.SetActive(true);
		Time.timeScale = 0;
	}

	public void Restart()
	{
		SceneManager.LoadSceneAsync("MainMenu");
		Time.timeScale = 1;
	}
}
