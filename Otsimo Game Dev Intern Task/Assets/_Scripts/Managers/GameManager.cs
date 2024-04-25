using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance { get; private set; }

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
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    public bool hasDrawn;

    public int canvasLayerCounter;
    public enum State
    {
        menu, paint
    }

    public enum Mode
    {
        pen, bucket, stamp, eraser, projectile
    }
    public Mode mode = new Mode();
    public State state = new State();

    private void Start()
    {
        canvasLayerCounter = 1;
    }

    private void OnApplicationQuit()
    {
        if (hasDrawn)
        {
            SaveManager.instance.SaveCurrentCanvas();
        }
    }
}
