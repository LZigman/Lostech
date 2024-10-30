using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject musicToggle, menuButtons, optionsButtons, pauseMenu, mainMenu, introSequence;
    public Slider musicSlider;
    [SerializeField] private Sprite musicOn, musicOff;

    private void Update()
    {
        if (AudioListener.volume == 0)
        {
            musicToggle.GetComponent<Image>().sprite = musicOff;
        }
        else
        {
            musicToggle.GetComponent<Image>().sprite = musicOn;
        }
    }

    public void NewGame()
    {
        mainMenu.SetActive(false);
        introSequence.SetActive(true);
    }
    
    public void OpenMainMenu()
    {
        optionsButtons.SetActive(false);
        menuButtons.SetActive(true);
        AudioManager.Instance.PlaySFX("MenuTheme");
    }

    public void OpenOptionsMenu()
    {
        if (menuButtons != null)
        {
            menuButtons.SetActive(false);
        }
        optionsButtons.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
    }
    
    public void ToggleMusic()
    {
        if (AudioListener.volume != 0)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    public void SoundVolume()
    {
        AudioListener.volume = musicSlider.value;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
