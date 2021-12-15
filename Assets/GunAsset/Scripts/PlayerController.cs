/* Класс персонажа, который котролирует собранную одежду и некоторые взаимодействия с внешней игрой*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private int weight = 0;

    [SerializeField] private int maxWeight = 10;

    [Header("Конвертирует 1 вес на указанный множетель")]
    [SerializeField] private int convectorMultiple = 10;

    [HideInInspector] public enum DressType { shlapa, boti, botySpilka, losini, yubka, bottomDress, topDress, other };

    private List<DressID> _topDress = new List<DressID>();
    private List<DressID> _bottomDress = new List<DressID>();

    [SerializeField] private RectTransform weightBarVisual;
    private float startVisualHeight;

    private GameObject currentYubka;
    private GameObject currentBoti;
    private GameObject currenShlapa;
    private GameObject currentBottomDress;
    [SerializeField] private GameObject defaultBoti;
    [SerializeField] private GameObject defaultSlapa;

    [SerializeField] private Text maximumText;

    [SerializeField] private GameObject Arrow;
    private Transform _ExitZone;

    [SerializeField] private Animator weightAnim;
    
    [SerializeField] private ParticleSystem cointSpawnSystem;
    [Header("Партикл при подборе одежды")]
    [SerializeField] private ParticleSystem whenDressUseParticalSystem;

    [Header("HaveDress")]
    [SerializeField] private DressID[] haveDress = new DressID[1];

    private void Start()
    {
        _ExitZone = FindObjectOfType<ExitZone>().transform;

        maximumText.gameObject.SetActive(true);
        maximumText.text = "";
        Arrow.SetActive(false);
        startVisualHeight = weightBarVisual.rect.width;
        weightBarVisual.sizeDelta = new Vector2(0, weightBarVisual.rect.height);
        currentBoti = defaultBoti;
    }

    private void FixedUpdate()
    {
        if (Arrow.activeInHierarchy)
            Arrow.transform.LookAt(_ExitZone);
    }

    public void GameStarted()
    {
        weight = 0;
    }

    public void GameEnded()
    {

    }

    [Header("Для Разработчиков: вкл. ограничение подбора активных вещей")]
    [SerializeField] private bool cantUse = false;

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameController.Instance.gameIsPlayed) return;
            
        if (weight == 0) return;

        if (other.GetComponent<ExitZone>())
        {
            weightAnim.SetTrigger("Idle");
            gameObject.GetComponent<CapsuleCollider>().height = 1.70f;

            //GameController.Instance.UpdatePoint(weight * convectorMultiple);
            StartCoroutine(DressOut(other.GetComponent<ExitZone>()));
            weight = 0;
            weightBarVisual.sizeDelta = new Vector2(0, weightBarVisual.rect.height);

            if (maximumText != null) maximumText.text = "";
            currenShlapa = null;
            currentYubka = null;
            defaultBoti.SetActive(true);
            currentBoti = defaultBoti;
            defaultSlapa.SetActive(true);
            Arrow.SetActive(false);
        }
    }

    private IEnumerator DressOut(ExitZone exitZone)
    {
        _topDress.Clear();
        _bottomDress.Clear();

        for (int i = 0; i < haveDress.Length; i++)
        {
            int j = haveDress.Length - 1 - i;
            if (haveDress[j]._dress != null)
            {
                if (haveDress[j]._dress.gameObject.activeInHierarchy)
                {
                    exitZone.BoxFlyToCar(gameObject.transform);
                    haveDress[j]._dress.gameObject.SetActive(false);
                }
            }
            else
                if (haveDress[j]._dressMeshRender.Length > 0)
                for (int x = 0; x < haveDress[j]._dressMeshRender.Length; x++)
                    if (haveDress[j]._dressMeshRender[x].gameObject.activeInHierarchy)
                    {
                        exitZone.BoxFlyToCar(gameObject.transform);
                        haveDress[j]._dressMeshRender[x].gameObject.SetActive(false);
                    }

            if (cointSpawnSystem != null) cointSpawnSystem.Play();
            yield return new WaitForSeconds(0.005f);
        }
    }
}

[System.Serializable]
public class DressID
{
    public SkinnedMeshRenderer _dress;
    public MeshRenderer[] _dressMeshRender;
    public int ID;
    public PlayerController.DressType dressType;
}