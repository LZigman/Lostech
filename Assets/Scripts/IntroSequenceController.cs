using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class IntroSequenceController : MonoBehaviour
{
	[Tooltip("What we want to be written with a typewriter effect")]
	[SerializeField] private string[] messages;

	[SerializeField] private GameObject promptText;

	[SerializeField] private TypewriterEffect typewriter;
	private int counter = 0;

	private void OnEnable()
	{
		foreach (var text in messages)
		{		
			StartCoroutine(StartTyping(text));
		}
	}

	private void Update()
	{
		if (counter != messages.Length) return;
		promptText.SetActive(true);
		if(Input.GetKeyDown(KeyCode.E))
		{
			SceneManager.LoadSceneAsync("Stage1");
		}
	}

	private IEnumerator StartTyping(string text)
	{
		while (typewriter.IsRunning)
		{
			yield return null;
		}
		typewriter.StartEffect(text);
		counter++;
		yield return null;
	}
}
