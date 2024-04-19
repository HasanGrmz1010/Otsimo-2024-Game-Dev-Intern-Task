using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class BucketHandler : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker FCP;
    [SerializeField] Camera mainCam;

    bool canFillCanvas = true;

    void Update()
    {
        if (Input.touchCount > 0 && GameManager.instance.mode == GameManager.Mode.bucket && canFillCanvas)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    
                    break;

                case TouchPhase.Moved:
                    
                    break;

                case TouchPhase.Stationary:
                    break;

                case TouchPhase.Ended:
                    GameObject currentClickedObj = EventSystem.current.currentSelectedGameObject;
                    if (currentClickedObj == null)
                    {
                        canFillCanvas = true;
                    }
                    else if (currentClickedObj.TryGetComponent<Button>(out Button but)) { canFillCanvas = false; return; }
                    else canFillCanvas = true;

                    mainCam.backgroundColor = FCP.color;
                    break;

                case TouchPhase.Canceled:
                    break;

                default:
                    break;
            }
        }
    }

    void AllowBucketFill()
    {
        canFillCanvas = true;
    }

    void DisallowBucketFill()
    {
        canFillCanvas = false;
    }

    private void OnEnable()
    {
        ButtonManager.onAnyMenuOpened += DisallowBucketFill;
        ButtonManager.onAllMenusClosed += AllowBucketFill;
    }

    private void OnDisable()
    {
        ButtonManager.onAnyMenuOpened -= DisallowBucketFill;
        ButtonManager.onAllMenusClosed -= AllowBucketFill;
    }
}
