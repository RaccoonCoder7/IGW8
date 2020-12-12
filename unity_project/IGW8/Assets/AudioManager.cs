using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    private AudioSource asBGM;
    private AudioSource asSFX;
    public AudioClip BGM;
    public AudioClip deadEyeSFX;
    public AudioClip gunshotSFX;
    public AudioClip reloadSFX;
    public AudioClip emptySFX;
    public AudioClip counterSFX;
    public AudioClip dashSFX;
    public AudioClip deathSFX;
    
    void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        asBGM = GetComponents<AudioSource>()[1];
        asSFX = GetComponents<AudioSource>()[0];
        PlayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopPlayingBGM()
    {
        //asBGM.Stop();
    }

    public void StopPlayingDeadEye()
    {
        asBGM.volume = .5f;
        asSFX.Stop();
    }

    public void PlayBGM()
    {
        asBGM.volume = .5f;
        asBGM.clip = BGM;
        asBGM.loop = true;
        asBGM.Play();
    }

    public void PlayDeadEye()
    {
        asBGM.volume = .1f;
        asSFX.clip = deadEyeSFX;
        asSFX.loop = true;
        asSFX.Play();
    }

    public void PlayGunshot()
    {
        asSFX.PlayOneShot(gunshotSFX);
    }

    public void PlayReload()
    {
        asSFX.PlayOneShot(reloadSFX);
    }

    public void PlayEmpty()
    {
        asSFX.PlayOneShot(emptySFX);
    }

    public void PlayDash()
    {
        asSFX.PlayOneShot(dashSFX);
    }

    public void PlayCounter()
    {
        asSFX.PlayOneShot(counterSFX);
    }
    
    public void PlayDeath()
    {
        asSFX.PlayOneShot(deathSFX);
    }
}
