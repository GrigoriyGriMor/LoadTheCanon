using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFireObjController : MonoBehaviour
{
    //Parameters
    [Header("Parameters")]
    [SerializeField] private int damage;
    [SerializeField] private float speed;

    [SerializeField] private GameObject visual;

    [SerializeField] private ParticleSystem contactParticle;

    void Update()
    {
        if (gameObject.activeInHierarchy)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GunController>())
        {
            other.GetComponent<GunController>().SetDamage(damage);
            visual.SetActive(false);
            if (contactParticle != null) contactParticle.Play();

            StartCoroutine(Deactive());
        }
    }

    private IEnumerator Deactive()
    {
        yield return new WaitForSeconds(1.5f);
        visual.SetActive(true);
        gameObject.SetActive(false);
    }

    [SerializeField] private float lifeTime = 5;
    private Coroutine timer;
    private void OnEnable()
    {
        if (timer != null) StopCoroutine(timer);
        timer = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(lifeTime);

        StartCoroutine(Deactive());
        timer = null;
    }
}
