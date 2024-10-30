using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
	[SerializeField] private GameObject endScreen;
	[SerializeField] private TextMeshProUGUI promptText;
	[SerializeField] private KeyCode keyToPress = KeyCode.E;
	[SerializeField] private GameObject doorCollider;
	[SerializeField] private bool isOpen;

	private void Start()
	{
		promptText.text = $"Press '{keyToPress}' key to activate.";
		promptText.enabled = false;
	}
	
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (!isOpen)
			{
				promptText.enabled = true;
				if (other.gameObject.GetComponent<Player>().isInteracting == true)
				{
					ShowEndScreen();
					other.gameObject.GetComponent<Player>().isInteracting = false;
                }
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			promptText.enabled = false;
		}
	}

	private void ShowEndScreen()
	{
		doorCollider.SetActive(false);
		isOpen = true;
		endScreen.SetActive(true);
		Time.timeScale = 0;
	}

	public void Restart()
	{
		SceneManager.LoadSceneAsync("Stage1");
	}
}
