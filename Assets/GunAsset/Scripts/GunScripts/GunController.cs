using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform firePos;

    //Lists
    private List<CartrigeSetting.CartrigeType> oboyma = new List<CartrigeSetting.CartrigeType>();
    private List<GameObject> rocketsPool = new List<GameObject>();
    private List<GameObject> machinegunsPool = new List<GameObject>();

    //Parameters
    [Header("Parameters")]
    [SerializeField] private int RocketAmount;
    [SerializeField] private int MachinegunAmount;
    [SerializeField] private float fireCoolDown = 0.5f;

    [SerializeField] private int health;
    [SerializeField] private ParticleSystem deathParticle;

    //References
    [Header("References")]
    [SerializeField] private GameObject rocket;
    [SerializeField] private GameObject machinegun;
    [SerializeField] private ParticleSystem gunShot;

    //Set up in Inspector
    public bool playerGun;

    [Header("CartrigeIn Setting")]
    [SerializeField] private Transform setPosition;

    [Header("Ракеты")]
    [SerializeField] private GameObject visualRocketChest;
    private List<GameObject> visualRocketPool = new List<GameObject>();

    [Header("Пулемет")]
    [SerializeField] private GameObject visualMinigunChest;
    private List<GameObject> visualMinigunPool = new List<GameObject>();

    [SerializeField] private float parcerSetObj = 1;

    void Start()
    {
        GenerateAmmo();

        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(visualRocketChest, setPosition.position, setPosition.rotation, setPosition);
            visualRocketPool.Add(go);
            go.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(visualMinigunChest, setPosition.position, setPosition.rotation, setPosition);
            visualMinigunPool.Add(go);
            go.SetActive(false);
        }
    }

    private bool die = false;
    void FixedUpdate()
    {
        //Fire if one or more ammo in oboyma
        if (!fire && oboyma.Count > 0)
            StartCoroutine(Fire());
    }

    //Load ammo from player
    public void LoadAmmo(CartrigeSetting.CartrigeType _ammoType)
    {
        oboyma.Add(_ammoType);

        switch (_ammoType)
        {
            case CartrigeSetting.CartrigeType.mashineGun :
                GameObject go = GetFree(visualMinigunPool);

                //if (go == null) break;

                go.SetActive(true);
                go.transform.position = new Vector3(setPosition.position.x, setPosition.position.y + (parcerSetObj * oboyma.Count), setPosition.position.z);
                break;

            case CartrigeSetting.CartrigeType.roket :
                GameObject go1 = GetFree(visualRocketPool);

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
                GameObject go = GetFree(rocketsPool);
                if (go != null)
                {
                    go.transform.position = firePos.position;
                    go.transform.rotation = firePos.rotation;
                    go.SetActive(true);
                }

                bool q = false;

                for (int j = 0; j < visualRocketPool.Count; j++)
                {
                    if (visualRocketPool[j].activeInHierarchy)
                    {
                        if (!q)
                        {
                            visualRocketPool[j].SetActive(false);
                            q = true;
                        }
                        else
                        {
                            visualRocketPool[j].transform.position = new Vector3(visualRocketPool[j].transform.position.x, visualRocketPool[j].transform.position.y - parcerSetObj,
                                visualRocketPool[j].transform.position.z);
                        }
                    }
                }
            }

            //Fire machinegun
            if (oboyma[i] == CartrigeSetting.CartrigeType.mashineGun)
            {
                GameObject go = GetFree(machinegunsPool);

                if (go != null)
                {
                    go.transform.position = firePos.position;
                    go.transform.rotation = firePos.rotation;
                    go.SetActive(true);
                }

                bool q = false;

                for (int j = 0; j < visualMinigunPool.Count; j++)
                {
                    if (visualMinigunPool[j].activeInHierarchy)
                    {
                        if (!q)
                        {
                            visualMinigunPool[j].SetActive(false);
                            q = true;
                        }
                        else
                        {
                            visualMinigunPool[j].transform.position = new Vector3(visualMinigunPool[j].transform.position.x, visualMinigunPool[j].transform.position.y - parcerSetObj,
                                visualMinigunPool[j].transform.position.z);
                        }
                    }
                }    
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
            GameObject obj = Instantiate(rocket);
            rocketsPool.Add(obj);
            obj.SetActive(false);
        }

        //Generate Bullets
        for (int i = 0; i < MachinegunAmount; i++)
        {
            GameObject obj = Instantiate(machinegun);
            machinegunsPool.Add(obj);
            obj.SetActive(false);
        }
    }

    private GameObject GetFree(List<GameObject> obj)
    {
        List<GameObject> targets = new List<GameObject>();
        for (int i = 0; i < obj.Count; i++)
            if (!obj[i].activeInHierarchy)
                return obj[i];

        if (obj.Count != 0)
            return obj[0];
        else
            return null;
    }

    public bool isFriendly()
    {
        if (playerGun)
            return true;
        else
            return false;
    }

    public void SetDamage(int _damage)
    {
        health -= _damage;

        //Death function
        if (!die && health <= 0)
            Death();
    }

    void Death()
    {
        die = true;
        if (deathParticle != null) deathParticle.Play();

        if (playerGun)
            GameController.Instance.EndGame(false);
        else
            GameController.Instance.EndGame(true);
    }
}
