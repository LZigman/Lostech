using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private Sound[] sounds;
    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private Sound mainMenuTheme, levelTheme;
    private AudioSource audioSource;
    private AudioSource mainMenuThemeAudioSource, levelThemeAudioSource;
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject temp = Instantiate(audioSourcePrefab, transform);
            temp.name = sounds[i].name;
            temp.GetComponent<AudioSource>().clip = sounds[i].clip;
            if (temp.name == "laser charge")
            {
                temp.GetComponent<AudioSource>().volume = 0.8f;
            }
        }
        if (mainMenuTheme != null)
        {
            GameObject temp = Instantiate(audioSourcePrefab, transform);
            temp.name = mainMenuTheme.name;
            mainMenuThemeAudioSource = temp.GetComponent<AudioSource>();
            mainMenuThemeAudioSource.clip = mainMenuTheme.clip;
            PlayMainMenuTheme();
        }
        if (levelTheme != null)
        {
            GameObject temp = Instantiate(audioSourcePrefab, transform);
            temp.name = mainMenuTheme.name;
            levelThemeAudioSource = temp.GetComponent<AudioSource>();
            levelThemeAudioSource.clip = levelTheme.clip;
            PlayLevelTheme();
        }
    }
    public void PlayMainMenuTheme()
    {
        mainMenuThemeAudioSource.loop = true;
        mainMenuThemeAudioSource.Play();
    }
    public void StopMainMenuTheme ()
    {
        mainMenuThemeAudioSource.Stop();
    }
    public void PlayLevelTheme ()
    {
        levelThemeAudioSource.loop = true;
        levelThemeAudioSource.Play();
    }
    public void PauseLevelTheme ()
    {
        levelThemeAudioSource.Play();
    }
    public void StopLevelTheme ()
    {
        levelThemeAudioSource.Stop();
    }
    public void PlaySFX (string name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == name)
            {
                if (transform.GetChild(i).GetComponent<AudioSource>().isPlaying == true)
                {
                    if (name == "player shoot" || name == "laser charge")
                    {
                        transform.GetChild(i).GetComponent<AudioSource>().Stop();
                        transform.GetChild(i).GetComponent<AudioSource>().Play();
                        return;
                    }
                }
                else if (transform.GetChild(i).GetComponent<AudioSource>().isPlaying == false)
                {
                    transform.GetChild(i).GetComponent<AudioSource>().Play();
                }
            }
        }
        Debug.Log("Sound not found!\nName: " + name);
    }
}
