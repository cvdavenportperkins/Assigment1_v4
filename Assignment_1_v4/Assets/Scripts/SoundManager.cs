using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager SoundMan;                           //assign SoundManager Singleton vaiable
    public AudioSource AS;           
    public AudioClip HitSFX, BoundarySFX, DecaySFX, PickUpSFX, ComboSFX, ScoreSFX, SpawnSFX, DespawnSFX; //create audioclip variables

    void Awake()                                                    //set singleton behavior to ensure a single persistent instance
    {
        if (SoundMan == null)
        {
            SoundMan = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)                       //assign PlaySound clip reference method and PlayOneShot behavior 
    {
        AS.PlayOneShot(clip);
    }
}