using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    FlexibleColorPicker FCP;

    [SerializeField] GameObject linePrefab;
    [SerializeField] Transform line_holder;
    [SerializeField] Camera mainCam;

    Line activeLine;
    Material activeColor;
    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GameObject newLine = Instantiate(linePrefab, mainCam.ScreenToWorldPoint(touch.position), Quaternion.identity, line_holder);
                    activeLine = newLine.GetComponent<Line>();
                    break;

                case TouchPhase.Moved:
                    Vector2 movedPos = mainCam.ScreenToWorldPoint(touch.position);
                    activeLine.UpdateLine(movedPos);
                    break;

                case TouchPhase.Stationary:
                    break;

                case TouchPhase.Ended:
                    activeLine.LockLineMaterial();
                    activeLine = null;
                    break;

                case TouchPhase.Canceled:
                    break;

                default:
                    break;
            }
        }
    }
}
