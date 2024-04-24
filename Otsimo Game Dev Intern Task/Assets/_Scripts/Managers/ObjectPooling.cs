using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    #region Singleton
    public static ObjectPooling instance { get; private set; }

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
            //DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion
    int poolSize;

    [SerializeField] FlexibleColorPicker FCP;

    private Queue<GameObject> AmmoPool = new Queue<GameObject>();
    public Transform ammoPoolTransform;
    [SerializeField] GameObject ammoPrefab;

    private void Start()
    {
        poolSize = 50;
        GameObject this_ammo;
        for (int i = 0; i < poolSize; i++)
        {
            this_ammo = Instantiate(ammoPrefab, ammoPoolTransform.position, Quaternion.identity, ammoPoolTransform);
            this_ammo.SetActive(false);
            AmmoPool.Enqueue(this_ammo);
        }
    }

    public void AddToAmmoPool(GameObject _obj)
    {
        if (_obj != null)
        {
            _obj.GetComponent<Ammo>().deactivated = true;
            _obj.SetActive(false);
            _obj.transform.position = transform.position;
            _obj.transform.rotation = Quaternion.identity;
            _obj.transform.SetParent(ammoPoolTransform);
            AmmoPool.Enqueue(_obj);
        }
        else return;
    }

    public void TakeFromPool(Vector3 pos, Quaternion rot, Vector3 _target)
    {
        if (AmmoPool.Count > 0)
        {
            GameObject _ammo = AmmoPool.Dequeue();
            _ammo.transform.position = pos;
            _ammo.transform.rotation = rot;
            _ammo.GetComponent<MeshRenderer>().material.color = FCP.color;
            _ammo.GetComponent<TrailRenderer>().material.color = FCP.color;
            _ammo.transform.SetParent(null);
            _ammo.SetActive(true);
            _ammo.GetComponent<Ammo>().target = _target;
            _ammo.GetComponent<Ammo>().deactivated = false;
        }
    }
}
