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

    public void PlayUI_SFX()
    {

    }

    public void PlayPainting_SFX()
    {

    }

    public void PlayMisc_SFX()
    {

    }
}
