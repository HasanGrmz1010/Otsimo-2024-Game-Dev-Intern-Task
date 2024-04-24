using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class BucketHandler : MonoBehaviour
{
    [SerializeField] GameObject canvasPlane;
    [SerializeField] Transform canvasItemHolder;

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
                    GameObject currentClickedObj = EventSystem.current.currentSelectedGameObject;
                    if (currentClickedObj == null)
                    {
                        canFillCanvas = true;
                    }
                    else if (currentClickedObj.TryGetComponent<Button>(out Button but))
                    {
                        canFillCanvas = false; return;
                    }
                    else canFillCanvas = true;
                    break;

                case TouchPhase.Ended:
                    if (canFillCanvas)
                    {
                        foreach (Transform item in SaveManager.instance.canvasItemHolder.transform)
                        {
                            Destroy(item.gameObject);
                        }
                        canvasPlane.GetComponent<MeshRenderer>().material.color = FCP.color;
                        GameManager.instance.canvasLayerCounter = 1;
                    }
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
