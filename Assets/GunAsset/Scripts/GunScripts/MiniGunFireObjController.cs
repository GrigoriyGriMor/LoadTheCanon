using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGunFireObjController : MonoBehaviour
{
    //Parameters
    [Header("Parameters")]
    [SerializeField] private int damage;
    [SerializeField] private float speed;

    void Update()
    {
        if (gameObject.activeInHierarchy)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
