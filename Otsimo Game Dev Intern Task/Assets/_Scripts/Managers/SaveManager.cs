using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using Newtonsoft.Json;
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

    [SerializeField] Material blankMat;
    [SerializeField] GameObject PenLine;
    [SerializeField] GameObject EraserLine;
    [SerializeField] GameObject Stamp;
    [SerializeField] GameObject Splash;

    public GameObject canvasItemHolder;
    [SerializeField] GameObject canvasPlane;

    ////////////////////////////////////////////////////////////////////
    Dictionary<int, List<Vector3>> lineVerticesPair = new Dictionary<int, List<Vector3>>();
    List<Color> lineColors = new List<Color>();
    List<int> lineSortingOrders = new List<int>();

    Dictionary<int, List<Vector3>> eraserVerticesPair = new Dictionary<int, List<Vector3>>();
    List<int> eraserSortingOrders = new List<int>();

    List<Vector3> stampPositions = new List<Vector3>();
    List<int> stampSortingOrders = new List<int>();

    List<Vector3> _splashPositions = new List<Vector3>();
    List<Color> _splashColors = new List<Color>();
    List<int> _splashSortingOrders = new List<int>();

    Color _canvasPlaneColor;
    bool hasDrawn;
    
    public void SaveCurrentCanvas()
    {
        int c1 = 0;
        int c2 = 0;

        #region FILL ALL THE DATA HOLDERS
        // fill all the information to coresponding data holders
        _canvasPlaneColor = canvasPlane.GetComponent<MeshRenderer>().material.color;
        foreach (Transform item in canvasItemHolder.transform)
        {
            if (item.CompareTag("line_pen"))// Pen Line Found
            {
                if (!lineVerticesPair.ContainsKey(c1))
                {
                    LineRenderer this_lr = item.GetComponent<LineRenderer>();
                    lineSortingOrders.Add(this_lr.sortingOrder);
                    lineColors.Add(this_lr.material.color);

                    List<Vector3> vertices = new List<Vector3>();
                    int vert_count = this_lr.positionCount;
                    for (int i = 0; i < vert_count; i++)
                    {
                        vertices.Add(this_lr.GetPosition(i));
                    }
                    lineVerticesPair.Add(c1, vertices);
                    c1++;
                }
            }

            else if (item.CompareTag("line_eraser"))// Eraser Line Found
            {
                if (!eraserVerticesPair.ContainsKey(c2))
                {
                    LineRenderer this_lr = item.GetComponent<LineRenderer>();
                    eraserSortingOrders.Add(this_lr.sortingOrder);

                    List<Vector3> vertices = new List<Vector3>();
                    int vert_count = this_lr.positionCount;
                    for (int i = 0; i < vert_count; i++)
                    {
                        vertices.Add(this_lr.GetPosition(i));
                    }
                    eraserVerticesPair.Add(c2, vertices);
                    c2++;
                }
            }

            else if (item.CompareTag("stamp"))
            {
                if (!stampPositions.Contains(item.localPosition))
                {
                    stampPositions.Add(item.localPosition);
                    stampSortingOrders.Add(item.GetComponent<SpriteRenderer>().sortingOrder);
                }
            }

            else if (item.CompareTag("splash"))
            {
                if (!_splashPositions.Contains(item.localPosition))
                {
                    _splashSortingOrders.Add(item.GetComponent<SpriteRenderer>().sortingOrder);
                    Color _thisColor = item.GetComponent<SpriteRenderer>().color;
                    _splashColors.Add(_thisColor);
                    _splashPositions.Add(item.localPosition);
                }
            }
        }
        #endregion

        hasDrawn = GameManager.instance.hasDrawn;
        PlayerPrefs.SetInt("HasDrawn", hasDrawn ? 1 : 0);

        PlayerPrefs.SetInt("LastLayerOrder", GameManager.instance.canvasLayerCounter);

        // convert to json and save to playerPrefs
        string linePair_json = JsonConvert.SerializeObject(lineVerticesPair,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("LinePair", linePair_json);

        List<string> serializedColors = new List<string>();
        foreach (Color color in lineColors)
        {
            string serializedColor = JsonConvert.SerializeObject(new { r = color.r,
                g = color.g,
                b = color.b,
                a = color.a });
            serializedColors.Add(serializedColor);
        }
        string lineColors_json = JsonConvert.SerializeObject(serializedColors);

        PlayerPrefs.SetString("LineColors", lineColors_json);

        string lineSorting_json = JsonConvert.SerializeObject(lineSortingOrders,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("LineOrders", lineSorting_json);

        string eraserPair_json = JsonConvert.SerializeObject(eraserVerticesPair,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("EraserPair", eraserPair_json);

        string eraserSorting_json = JsonConvert.SerializeObject(eraserSortingOrders,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("EraserOrders", eraserSorting_json);

        string stampPos_json = JsonConvert.SerializeObject(stampPositions,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("Stamp", stampPos_json);

        string stampOrder_json = JsonConvert.SerializeObject(stampSortingOrders,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("StampOrders", stampOrder_json);

        string _splashPos_json = JsonConvert.SerializeObject(_splashPositions,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("Splash", _splashPos_json);

        List<string> _serializedColors = new List<string>();
        foreach (Color color in _splashColors)
        {
            string serializedColor = JsonConvert.SerializeObject(new { r = color.r,
                g = color.g,
                b = color.b,
                a = color.a });
            _serializedColors.Add(serializedColor);
        }
        string _splashColors_json = JsonConvert.SerializeObject(_serializedColors);

        PlayerPrefs.SetString("SplashColors", _splashColors_json);

        string _splashOrder_json = JsonConvert.SerializeObject(_splashSortingOrders,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        PlayerPrefs.SetString("SplashOrders", _splashOrder_json);

        string serializedPlaneColor = JsonConvert.SerializeObject(new { r = _canvasPlaneColor.r,
            g = _canvasPlaneColor.g,
            b = _canvasPlaneColor.b,
            a = _canvasPlaneColor.a });
        string canvasPlaneColor_json = JsonConvert.SerializeObject(serializedPlaneColor);

        PlayerPrefs.SetString("PlaneColor", canvasPlaneColor_json);
        PlayerPrefs.Save();
    }

    public void LoadSavedCanvas()
    {
        int loadedLastOrder = PlayerPrefs.GetInt("LastLayerOrder");

        if (PlayerPrefs.HasKey("LinePair"))
        {
            string loadedLinePair_json = PlayerPrefs.GetString("LinePair");
            Dictionary<int, List<Vector3>> loadedLineVertPair = JsonConvert.DeserializeObject<Dictionary<int, List<Vector3>>>(loadedLinePair_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            string loadedLineColors_json = PlayerPrefs.GetString("LineColors");
            List<string> serializedColors = JsonConvert.DeserializeObject<List<string>>(loadedLineColors_json);
            List<Color> loadedLineColorList = new List<Color>();
            foreach (string s_color in serializedColors)
            {
                var colorData = JsonConvert.DeserializeAnonymousType(s_color, new { r = default(float), g = default(float), b = default(float), a = default(float) });
                Color loadedColor = new Color(colorData.r, colorData.g, colorData.b, colorData.a);
                loadedLineColorList.Add(loadedColor);
            }

            string loadedLineSorting_json = PlayerPrefs.GetString("LineOrders");
            List<int> loadedLineSortingList = JsonConvert.DeserializeObject<List<int>>(loadedLineSorting_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            //==================================================================================================================

            for (int i = 0; i < loadedLineVertPair.Keys.Count; i++)
            {
                Color _thisLineColor = loadedLineColorList[i];
                GameObject _thisLine = Instantiate(PenLine, canvasItemHolder.transform);
                LineRenderer _thisLR = _thisLine.GetComponent<LineRenderer>();
                _thisLR.material.color = _thisLineColor;
                _thisLR.sortingOrder = loadedLineSortingList[i];

                _thisLR.positionCount = loadedLineVertPair[i].Count;
                for (int j = 0; j < loadedLineVertPair[i].Count; j++)
                {
                    _thisLR.SetPosition(j, loadedLineVertPair[i][j]);
                }
            }
        }

        if (PlayerPrefs.HasKey("EraserPair"))
        {
            string loadedEraserPair_json = PlayerPrefs.GetString("EraserPair");
            Dictionary<int, List<Vector3>> loadedEraserVertPair = JsonConvert.DeserializeObject<Dictionary<int, List<Vector3>>>(loadedEraserPair_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            string loadedEraserSorting_json = PlayerPrefs.GetString("EraserOrders");
            List<int> loadedEraserSortingList = JsonConvert.DeserializeObject<List<int>>(loadedEraserSorting_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            //==================================================================================================

            for (int i = 0; i < loadedEraserVertPair.Keys.Count; i++)
            {
                GameObject _thisEraserLine = Instantiate(EraserLine, canvasItemHolder.transform);
                LineRenderer _thisLR = _thisEraserLine.GetComponent<LineRenderer>();
                _thisLR.material.color = Color.white;
                _thisLR.sortingOrder = loadedEraserSortingList[i];

                _thisLR.positionCount = loadedEraserVertPair[i].Count;
                for (int j = 0; j < loadedEraserVertPair[i].Count; j++)
                {
                    _thisLR.SetPosition(j, loadedEraserVertPair[i][j]);
                }
            }
        }

        if (PlayerPrefs.HasKey("Stamp"))
        {
            string loadedStampPos_json = PlayerPrefs.GetString("Stamp");
            List<Vector3> loadedStampList = JsonConvert.DeserializeObject<List<Vector3>>(loadedStampPos_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            string loadedStampSorting_json = PlayerPrefs.GetString("StampOrders");
            List<int> loadedStampSortingList = JsonConvert.DeserializeObject<List<int>>(loadedStampSorting_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            //=====================================================================================================

            for (int i = 0; i < loadedStampList.Count; i++)
            {
                GameObject _thisStamp = Instantiate(Stamp, canvasItemHolder.transform);
                _thisStamp.transform.localPosition = loadedStampList[i];
                SpriteRenderer _this_sr = _thisStamp.GetComponent<SpriteRenderer>();
                _this_sr.sortingOrder = loadedStampSortingList[i];
            }
        }

        if (PlayerPrefs.HasKey("Splash"))
        {
            string loadedSplashPos_json = PlayerPrefs.GetString("Splash");
            List<Vector3> loadedSplashList = JsonConvert.DeserializeObject<List<Vector3>>(loadedSplashPos_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            string loadedSplashColors_json = PlayerPrefs.GetString("SplashColors");
            List<string> _serializedColors = JsonConvert.DeserializeObject<List<string>>(loadedSplashColors_json);
            List<Color> loadedSplashColorList = new List<Color>();
            foreach (string s_color in _serializedColors)
            {
                var colorData = JsonConvert.DeserializeAnonymousType(s_color, new { r = default(float), g = default(float), b = default(float), a = default(float) });
                Color loadedColor = new Color(colorData.r, colorData.g, colorData.b, colorData.a);
                loadedSplashColorList.Add(loadedColor);
            }

            string loadedSplashSorting_json = PlayerPrefs.GetString("SplashOrders");
            List<int> loadedSplashSortingList = JsonConvert.DeserializeObject<List<int>>(loadedSplashSorting_json,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            //=================================================================================================

            for (int i = 0; i < loadedSplashList.Count; i++)
            {
                GameObject _thisSplash = Instantiate(Splash, canvasItemHolder.transform);
                _thisSplash.transform.localPosition = loadedSplashList[i];
                SpriteRenderer _this_sr = _thisSplash.GetComponent<SpriteRenderer>();
                _this_sr.color = loadedSplashColorList[i];
                _this_sr.sortingOrder = loadedSplashSortingList[i];
            }
        } 

        if (PlayerPrefs.HasKey("PlaneColor"))
        {
            string loadedCanvasColor_json = PlayerPrefs.GetString("PlaneColor");
            string serializedPlaneColor = JsonConvert.DeserializeObject<string>(loadedCanvasColor_json);
            var planeColorData = JsonConvert.DeserializeAnonymousType(serializedPlaneColor, new { r = default(float), g = default(float), b = default(float), a = default(float) });
            Color loadedCanvasPlaneColor = new Color(planeColorData.r, planeColorData.g, planeColorData.b, planeColorData.a);
            canvasPlane.GetComponent<MeshRenderer>().material.color = loadedCanvasPlaneColor;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        GameManager.instance.canvasLayerCounter = loadedLastOrder;
    }

    public void LoadDrawnData()
    {
        if (PlayerPrefs.HasKey("HasDrawn"))
        {
            GameManager.instance.hasDrawn = PlayerPrefs.GetInt("HasDrawn") == 1 ? true : false;
        }
    }
}
