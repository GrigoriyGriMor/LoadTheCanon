using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunController : MonoBehaviour
{
    //Lists
    [SerializeField] private List<CartrigeSetting.CartrigeType> oboyma = new List<CartrigeSetting.CartrigeType>();
    [SerializeField] private List<GameObject> rockets = new List<GameObject>();
    [SerializeField] private List<GameObject> machineguns = new List<GameObject>();

    [SerializeField] private Transform firePos;
    
    //Parameters
    [Header("Parameters")]
    [SerializeField] private int RocketAmount;
    [SerializeField] private int MachinegunAmount;
    [SerializeField] private int health;
    [SerializeField] private float fireCoolDown = 0.5f;

    //References
    [Header("References")]
    [SerializeField] private GameObject rocket;
    [SerializeField] private GameObject machinegun;
    [SerializeField] private ParticleSystem gunShot;

    //Set up in Inspector
    public bool playerGun;

    [Header("CartrigeIn Setting")]
    [SerializeField] private Transform setPosition;
    [SerializeField] private GameObject visualRocketChest;
    [SerializeField] private GameObject visualMinigunChest;
    [SerializeField] private List<GameObject> visualRocketList = new List<GameObject>();
    [SerializeField] private List<GameObject> visualMinigunList = new List<GameObject>();

    [SerializeField] private float parcerSetObj = 1;

    void Start()
    {
        if (RocketAmount > 0 && MachinegunAmount > 0)
            GenerateAmmo();

        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(visualRocketChest, setPosition.position, setPosition.rotation, setPosition);
            visualRocketList.Add(go);
            go.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(visualMinigunChest, setPosition.position, setPosition.rotation, setPosition);
            visualMinigunList.Add(go);
            go.SetActive(false);
        }
    }

    private bool die = false;
    void FixedUpdate()
    {
        //Fire if one or more ammo in oboyma
        if (!fire && oboyma.Count > 0)
            StartCoroutine(Fire());

        //Death function
        if (!die && health <= 0)
        {
            Death();
            die = true;
        }
    }

    public bool isFriendly()
    {
        if (playerGun)
            return true;
        else
            return false;
    }

    //Load ammo from player
    public void LoadAmmo(CartrigeSetting.CartrigeType _ammoType)
    {
        oboyma.Add(_ammoType);

        switch (_ammoType)
        {
            case CartrigeSetting.CartrigeType.mashineGun:
                GameObject go = GetFree(visualMinigunList);

                //if (go == null) break;

                go.SetActive(true);
                go.transform.position = new Vector3(setPosition.position.x, setPosition.position.y + (parcerSetObj * oboyma.Count), setPosition.position.z);
                break;

            case CartrigeSetting.CartrigeType.roket:
                GameObject go1 = GetFree(visualRocketList);

               // if (go1 == null) break;

                go1.SetActive(true);
                go1.transform.position = new Vector3(setPosition.position.x, setPosition.position.y + (parcerSetObj * oboyma.Count), setPosition.position.z);
                break;
        }
    }

    private bool fire = false;
    //Fire sequence
    IEnumerator Fire()
    {
        fire = true;
        for (int i = 0; i < oboyma.Count; i++)
        {
            //Fire rocket
            if (oboyma[i] == CartrigeSetting.CartrigeType.roket)
            {
                //Placing rocket on the barrel
                GameObject go = GetFree(rockets);
                if (go != null)
                {
                    go.transform.position = firePos.position;
                    go.SetActive(true);
                }

                for (int j = 0; j < visualRocketList.Count; j ++)
                    if (visualRocketList[i].activeInHierarchy)
                        visualRocketList[i].SetActive(false);
            }

            //Fire machinegun
            if (oboyma[i] == CartrigeSetting.CartrigeType.mashineGun)
            {
                GameObject go = GetFree(machineguns);

                if (go != null)
                {
                    go.transform.position = firePos.position;
                    go.SetActive(true);
                }

                for (int j = 0; j < visualMinigunList.Count; j++)
                    if (visualMinigunList[i].activeInHierarchy)
                        visualMinigunList[i].SetActive(false);
            }

            if (gunShot != null) gunShot.Play();
            oboyma.RemoveAt(0);

            yield return new WaitForSeconds(fireCoolDown);
        }

        fire = false;
    }

    //Ammo generator
    private void GenerateAmmo()
    {
        //Generate Rockets
        for (int i = 0; i < RocketAmount; i++)
        {
            var obj = Instantiate(rocket);
            rockets.Add(obj);
            obj.SetActive(false);
        }

        //Generate Bullets
        for (int i = 0; i < MachinegunAmount; i++)
        {
            var obj = Instantiate(machinegun);
            machineguns.Add(obj);
            obj.SetActive(false);
        }
    }

    private GameObject GetFree(List<GameObject> obj)
    {
        List<GameObject> targets = new List<GameObject>();
        for (int i = 0; i < obj.Count; i++)
            if (!obj[i].gameObject.activeInHierarchy)
                return obj[i];

        return null;
    }


    void Death()
    {
        if (playerGun)
            GameController.Instance.EndGame(false);
        else
            GameController.Instance.EndGame(true);
    }
}
