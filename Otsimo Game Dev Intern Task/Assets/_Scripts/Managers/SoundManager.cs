using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    [SerializeField] AudioSource UI_Source;
    [SerializeField] AudioSource PaintSource;
    [SerializeField] AudioSource Misc_Source;

    [Header("______________________________________________________________________")]
    [SerializeField] List<AudioClip> UI_soundClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> Gun_soundClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> Paint_soundClips = new List<AudioClip>();

    private void Start()
    {
        UI_Source.volume = 1f; UI_Source.pitch = 1f;
        PaintSource.volume = 1f; PaintSource.pitch = 1f;
        Misc_Source.volume = 1f; Misc_Source.pitch = 1f;
    }

    public void PlayUI_SFX(string audio)
    {
        switch (audio)
        {
            case "button_menu":
                UI_Source.PlayOneShot(UI_soundClips[0]);
                break;
            
            case "button_paint":
                UI_Source.volume = 0.5f;
                UI_Source.PlayOneShot(UI_soundClips[1]);
                break;

            case "button_tool":
                UI_Source.volume = 0.5f;
                UI_Source.PlayOneShot(UI_soundClips[2]);
                break;

            default:
                break;
        }
    }

    public void PlayPainting_SFX(string audio)
    {
        switch (audio)
        {
            case "stamp":
                PaintSource.pitch = 1.25f;
                PaintSource.PlayOneShot(Paint_soundClips[0]);
                break;
            
            case "eraser":
                PaintSource.pitch = 1f;
                PaintSource.PlayOneShot(Paint_soundClips[1]);
                break;

            case "saving":
                PaintSource.pitch = 1f;
                PaintSource.PlayOneShot(Paint_soundClips[2]);
                break;
            
            default:
                break;
        }
    }

    public void PlayMisc_SFX(string audio)
    {
        switch (audio)
        {
            case "gun_pop":
                Misc_Source.PlayOneShot(Gun_soundClips[0]);
                break;

            case "ammo_pop":
                Misc_Source.PlayOneShot(Gun_soundClips[1]);
                break;
            
            default:
                break;
        }
    }
}
