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

    [SerializeField] gameData gameData;

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
        if (gameData.hasDrawn)
        {
            for (int i = 0; i < gameData.lineVerticePairs.Count; i++)
            {
                foreach (Vector3 item in gameData.lineVerticePairs[i])
                {
                    print(item.x + " " + item.y + " " + item.z + "\n");
                }
            }
            print("======================================================");
            for (int i = 0; i < gameData.eraserVerticePairs.Count; i++)
            {
                foreach (Vector3 item in gameData.eraserVerticePairs[i])
                {
                    print(item.x + " " + item.y + " " + item.z + "\n");
                }
            }
        }
    }
}
