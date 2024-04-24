using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LineGenerator : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker FCP;

    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject eraserPrefab;
    //[SerializeField] Transform canvasItemHolder;
    [SerializeField] Camera mainCam;

    Line activeLine;
    [SerializeField] Material activeColor;

    bool canGenerateLines, canErase;
    private void Start()
    {
        canGenerateLines = true; canErase = true;
    }

    void Update()
    {
        if (GameManager.instance.state == GameManager.State.paint)
        {
            PenDrawing_Handle();
            Eraser_Handle();
        }
    }

    /*
     * This function handles generating a line through user's input with touching on the screen.
     */
    void PenDrawing_Handle()
    {
        if (Input.touchCount > 0 && canGenerateLines && GameManager.instance.mode == GameManager.Mode.pen)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GameObject currentClickedObj = EventSystem.current.currentSelectedGameObject;
                    if (currentClickedObj == null)
                    {
                        canGenerateLines = true;
                    }
                    else if (currentClickedObj.TryGetComponent<Button>(out Button but)) { canGenerateLines = false; return; }
                    else canGenerateLines = true;

                    activeColor.color = FCP.color;
                    GameObject newLine = Instantiate(linePrefab, mainCam.ScreenToWorldPoint(touch.position), Quaternion.identity, SaveManager.instance.canvasItemHolder.transform);
                    int layer = GameManager.instance.canvasLayerCounter++;
                    newLine.GetComponent<LineRenderer>().sortingOrder = layer;
                    activeLine = newLine.GetComponent<Line>();
                    break;

                case TouchPhase.Moved:
                    Vector2 movedPos = mainCam.ScreenToWorldPoint(touch.position);
                    activeLine.UpdateLine(movedPos);
                    break;

                case TouchPhase.Ended:
                    if (canGenerateLines && activeLine != null)
                    {
                        activeLine.LockLineMaterial();
                        activeLine = null;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    /*
     * This function handles erasing effect with generating white lines. (same color as background)
     */
    void Eraser_Handle()
    {
        if (Input.touchCount > 0 && canErase && GameManager.instance.mode == GameManager.Mode.eraser)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GameObject currentClickedObj = EventSystem.current.currentSelectedGameObject;
                    if (currentClickedObj == null)
                    {
                        canErase = true;
                    }
                    else if (currentClickedObj.TryGetComponent<Button>(out Button but)) { canErase = false; return; }
                    else canErase = true;

                    GameObject newLine = Instantiate(eraserPrefab, mainCam.ScreenToWorldPoint(touch.position), Quaternion.identity, SaveManager.instance.canvasItemHolder.transform);
                    int layer = GameManager.instance.canvasLayerCounter++;
                    newLine.GetComponent<LineRenderer>().sortingOrder = layer;
                    activeLine = newLine.GetComponent<Line>();
                    break;

                case TouchPhase.Moved:
                    Vector2 movedPos = mainCam.ScreenToWorldPoint(touch.position);
                    activeLine.UpdateLine(movedPos);
                    break;

                case TouchPhase.Stationary:
                    break;

                case TouchPhase.Ended:
                    if (canErase)
                    {
                        activeLine = null;
                    }
                    break;

                case TouchPhase.Canceled:
                    break;

                default:
                    break;
            }
        }
    }

    /*
     * EVENT FUNCTIONS WITH SUBSCRIBTIONS
     */

    public void SetLineGenerating(bool _val)
    {
        canGenerateLines = _val;
    }

    private void AllowGeneratingLines()
    {
        canGenerateLines = true;
        canErase = true;
    }

    private void DisallowGeneratingLines()
    {
        canGenerateLines = false;
        canErase = false;
    }

    private void OnEnable()
    {
        ButtonManager.onAnyMenuOpened += DisallowGeneratingLines;
        ButtonManager.onAllMenusClosed += AllowGeneratingLines;
    }

    private void OnDisable()
    {
        ButtonManager.onAnyMenuOpened -= DisallowGeneratingLines;
        ButtonManager.onAllMenusClosed -= AllowGeneratingLines;
    }
}
