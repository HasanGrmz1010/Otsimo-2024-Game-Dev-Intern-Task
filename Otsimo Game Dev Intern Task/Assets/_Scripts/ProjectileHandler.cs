using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProjectileHandler : MonoBehaviour
{
    [SerializeField] FlexibleColorPicker FCP;
    [SerializeField] GameObject ammoPrefab;
    
    [SerializeField] Camera mainCam;
    [SerializeField] LayerMask layerMask;
    private RaycastHit hit;

    bool canShoot;

    private void Start()
    {
        canShoot = false;
    }

    private void Update()
    {
        if (Input.touchCount > 0 && GameManager.instance.mode == GameManager.Mode.projectile && canShoot)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GameObject currentClickedObj = EventSystem.current.currentSelectedGameObject;
                    if (currentClickedObj == null) canShoot = true;
                    else if (currentClickedObj.TryGetComponent<Button>(out Button but)) canShoot = false;
                    else canShoot = true;

                    Ray ray = mainCam.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hit, 1000, layerMask))
                    {
                        if (hit.collider.CompareTag("CanvasPlane") && canShoot)
                        {
                            transform.LookAt(hit.point);

                            ObjectPooling.instance.TakeFromPool(transform.position, Quaternion.identity, hit.point);
                        }
                    }
                    
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Stationary:
                    break;

                case TouchPhase.Ended:
                    break;

                case TouchPhase.Canceled:
                    break;

                default:
                    break;
            }
        }
    }

    void AllowShooting()
    {
        canShoot = true;
    }

    void DisallowShooting()
    {
        canShoot = false;
    }

    private void OnEnable()
    {
        ButtonManager.onAnyMenuOpened += DisallowShooting;
        ButtonManager.onAllMenusClosed += AllowShooting;
    }

    private void OnDisable()
    {
        ButtonManager.onAnyMenuOpened -= DisallowShooting;
        ButtonManager.onAllMenusClosed -= AllowShooting;
    }
}
