using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newGameData", menuName = "Game Data")]
public class gameData : ScriptableObject
{
    [SerializeField] GameObject stamp;
    [SerializeField] GameObject splash;

    public int lastSortLayer;
    public bool hasDrawn;

    public List<Color> lineColors = new List<Color>();
    public List<Color> splashColors = new List<Color>();

    public Dictionary<int, List<Vector3>> lineVerticePairs = new Dictionary<int, List<Vector3>>();
    public Dictionary<int, List<Vector3>> eraserVerticePairs = new Dictionary<int, List<Vector3>>();

    public List<Vector3> stampPosList = new List<Vector3>();
    public List<Vector3> splash_PosList = new List<Vector3>();
}
