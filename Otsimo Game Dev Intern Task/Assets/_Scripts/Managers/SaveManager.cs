using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Singleton
    public static SaveManager instance { get; private set; }

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
    [SerializeField] Material blankMat;

    [SerializeField] GameObject PenLine;
    [SerializeField] GameObject EraserLine;

    public GameObject canvasItemHolder;

    public void SaveCurrentCanvas()
    {
        gameData.lastSortLayer = GameManager.instance.canvasLayerCounter;

        gameData.lineColors.Clear();
        gameData.splashColors.Clear();
        
        foreach (Transform item in canvasItemHolder.transform)
        {
            if (item.CompareTag("line_pen"))
            {
                Color this_color = item.GetComponent<LineRenderer>().material.color;
                gameData.lineColors.Add(this_color);
            }
            else if (item.CompareTag("splash"))
            {
                Color splash_color = item.GetComponent<SpriteRenderer>().material.color;
                gameData.splashColors.Add(splash_color);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        gameData.lineVerticePairs.Clear();
        gameData.eraserVerticePairs.Clear();
        gameData.stampPosList.Clear();
        gameData.splash_PosList.Clear();

        int ct1 = 0;
        int ct2 = 0;

        foreach (Transform item in canvasItemHolder.transform)
        {
            if (item.CompareTag("line_pen"))
            {
                List<Vector3> _LineVertices = new List<Vector3>();
                int verticesCount = item.GetComponent<LineRenderer>().positionCount;

                for (int i = 0; i < verticesCount; i++)
                {
                    _LineVertices.Add(item.GetComponent<LineRenderer>().GetPosition(i));
                }

                gameData.lineVerticePairs.Add(ct1, _LineVertices);

                ct1++;
            }
            else if (item.CompareTag("line_eraser"))
            {
                List<Vector3> _EraserVertices = new List<Vector3>();
                int verticesCount = item.GetComponent<LineRenderer>().positionCount;

                for (int i = 0; i < verticesCount; i++)
                {
                    _EraserVertices.Add(item.GetComponent<LineRenderer>().GetPosition(i));
                }

                gameData.eraserVerticePairs.Add(ct1, _EraserVertices);

                ct2++;
            }
            else if (item.CompareTag("stamp"))
            {
                gameData.stampPosList.Add(item.transform.localPosition);
            }
            else if (item.CompareTag("splash"))
            {
                gameData.splash_PosList.Add(item.transform.localPosition);
            }
        }
    }

    public void LoadSavedCanvas()
    {
        GameManager.instance.canvasLayerCounter = gameData.lastSortLayer;

        if (gameData.lineVerticePairs.ContainsKey(0))
        {
            for (int i = 0; i < gameData.lineVerticePairs.Keys.Count; i++)
            {
                GameObject _line = Instantiate(PenLine, canvasItemHolder.transform);
                LineRenderer _thisLR = _line.GetComponent<LineRenderer>();
                Material _newMat = new Material(blankMat);

                _thisLR.material = _newMat;
                _newMat.color = gameData.lineColors[i];
                _thisLR.sortingOrder = i + 1;
                //_thisLR.startColor = gameData.lineColors[i];
                //_thisLR.endColor = gameData.lineColors[i];
                int ct = 0;
                foreach (Vector3 vec in gameData.lineVerticePairs[i])
                {
                    _thisLR.positionCount = gameData.lineVerticePairs[i].Count;
                    _thisLR.SetPosition(ct, vec);
                    ct++;
                }
            }
        }
    }


}
