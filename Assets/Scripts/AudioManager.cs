using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private Sound[] sounds;
    [SerializeField] private GameObject audioSourcePrefab;
    private AudioSource audioSource;
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject temp = Instantiate(audioSourcePrefab, transform);
            temp.name = sounds[i].name;
            temp.GetComponent<AudioSource>().clip = sounds[i].clip;
        }
    }

    public void PlaySFX (string name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == name)
            {
                if (transform.GetChild(i).GetComponent<AudioSource>().isPlaying == false)
                {
                    transform.GetChild(i).GetComponent<AudioSource>().Play();
                    return;
                }
            }
        }
        Debug.Log("Sound not found!\nName: " + name);
    }
}