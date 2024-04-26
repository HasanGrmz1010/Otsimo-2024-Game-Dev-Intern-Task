using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] GameObject splash_img;
    [SerializeField] float speed;
    public Vector3 target;
    Transform canvas_item_holder;
    
    Camera mainCam;
    public bool deactivated;

    private void Start()
    {
        mainCam = Camera.main;
        canvas_item_holder = GameObject.FindGameObjectWithTag("CanvasHolder").transform;

        deactivated = false;
        speed = 40f;
    }

    private void Update()
    {
        if (!deactivated)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 6)
        {
            int layer = GameManager.instance.canvasLayerCounter++;

            GameObject _splash = Instantiate(splash_img, transform.position, Quaternion.identity);
            _splash.GetComponent<SpriteRenderer>().sortingOrder = layer;
            _splash.GetComponent<SpriteRenderer>().color = GetComponent<MeshRenderer>().material.color;
            _splash.transform.SetParent(canvas_item_holder);
            float _scale = Random.Range(.2f, .25f);
            _splash.transform.localScale = new Vector3(_scale, _scale, 1f);
            _splash.transform.Rotate(0, 0, Random.Range(0f, 360f));

            SoundManager.instance.PlayMisc_SFX("ammo_pop");
            mainCam.DOShakePosition(.1f, .25f, 10, 90f, true, ShakeRandomnessMode.Harmonic);
            ObjectPooling.instance.AddToAmmoPool(gameObject);
        }
    }
}
