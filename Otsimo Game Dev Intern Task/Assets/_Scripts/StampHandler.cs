using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StampHandler : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject stampSprite;
    //[SerializeField] Transform canvasItemHolder;

    bool canStamp = true;
    private void Update()
    {
        if (Input.touchCount > 0 && GameManager.instance.mode == GameManager.Mode.stamp)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GameObject currentClickedObj = EventSystem.current.currentSelectedGameObject;
                    if (currentClickedObj == null) return;
                    else if (currentClickedObj.TryGetComponent<Button>(out Button but)) canStamp = false;
                    else canStamp = true;
                    break;

                case TouchPhase.Ended:
                    if (canStamp)
                    {
                        GameObject newSprite = Instantiate(stampSprite, mainCam.ScreenToWorldPoint(touch.position) + Vector3.forward, Quaternion.identity, SaveManager.instance.canvasItemHolder.transform);
                        int layer = GameManager.instance.canvasLayerCounter++;
                        newSprite.GetComponent<SpriteRenderer>().sortingOrder = layer;
                        newSprite.transform.DOPunchScale(Vector3.one / 10, .2f, 1, .25f);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    void AllowStamp()
    {
        canStamp = true;
    }

    void DisallowStamp()
    {
        canStamp = false;
    }

    private void OnEnable()
    {
        ButtonManager.onAnyMenuOpened += DisallowStamp;
        ButtonManager.onAllMenusClosed += AllowStamp;
    }

    private void OnDisable()
    {
        ButtonManager.onAnyMenuOpened -= DisallowStamp;
        ButtonManager.onAllMenusClosed -= AllowStamp;
    }
}
