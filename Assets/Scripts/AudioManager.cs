using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private Sound[] sounds;
    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound (string name)
    {
        if (sounds[0].name == name)
        {
            sounds[0].source.Play();
        }
    }
}
