/* Класс управления ботами, можно настроить скорость, собираемый вес и минимальную дистанцию */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompetitorController : MonoBehaviour
{
    private NavMeshAgent AI;

    [Header("AI Setting")]
    [SerializeField] private float speed = 5;
    [SerializeField] private int maxWeight = 5;
    [SerializeField] private Transform exitZone;
    [SerializeField] private float minDistance = 2;

    [SerializeField] private Transform target;

    [SerializeField] private Animator anim;

    [Header("CartrigeIn Setting")]
    [SerializeField] private Transform setPosition;
    [SerializeField] private GameObject visualFireObj;
    private List<CartrigeSetting> cartrigeList = new List<CartrigeSetting>();
    [SerializeField] private float parcerSetObj = 1;
    private int activeObjCount = 0;

    private void Start()
    {
        AI = GetComponent<NavMeshAgent>();
        activeObjCount = 0;
        AI.speed = 0;

        for (int i = 0; i < 20; i++)
        {
            CartrigeSetting setting = new CartrigeSetting();
            setting.visual = Instantiate(visualFireObj, new Vector3(setPosition.position.x, setPosition.position.y + (i * parcerSetObj), setPosition.position.z), setPosition.rotation, setPosition);
            cartrigeList.Add(setting);
            setting.visual.SetActive(false);
        }
    }

    public void GameStart()
    {
        activeObjCount = 0;

        target = WardrobeCollection.Instance.ChekNewTarget();
        AI.destination = target.position;
        AI.speed = speed;
        anim.SetBool("Run", true);
    }

    public void EndGame(bool win)
    {
        AI.speed = speed;

        if (win)
            anim.SetBool("LoseGame", true);
        else
            anim.SetBool("WinGame", true);

        anim.SetBool("Run", false);
    }

    [SerializeField] private Material electrickMat;
    [SerializeField] private Material roketMat;

    public void UpgradeCartrigeActiveCount(CartrigeSetting.CartrigeType _type = CartrigeSetting.CartrigeType.mashineGun)
    {
        cartrigeList[activeObjCount].visual.SetActive(true);
                if (_type == CartrigeSetting.CartrigeType.mashineGun) 
            cartrigeList[activeObjCount].visual.transform.GetChild(0).GetComponent<MeshRenderer>().material = electrickMat;
        else
            cartrigeList[activeObjCount].visual.transform.GetChild(0).GetComponent<MeshRenderer>().material = roketMat;
        
        cartrigeList[activeObjCount].type = _type;
        activeObjCount += 1;

        if (activeObjCount >= maxWeight)
            target = exitZone;
        else
            target = WardrobeCollection.Instance.ChekNewTarget();

        AI.destination = target.position;
    }

    private float time = 0;
    private void FixedUpdate()
    {
        if (!GameController.Instance.gameIsPlayed)
        {
            AI.speed = 0;
            return;
        }

        if (target != null && Vector3.Distance(gameObject.transform.position, target.transform.position) < minDistance)
        {
            target = WardrobeCollection.Instance.ChekNewTarget();
            AI.destination = target.position;
            time = 0;
        }
        else
        {
            if ((target != null && target != exitZone) && time >= 5f)
            {
                target = WardrobeCollection.Instance.ChekNewTarget();
                AI.destination = target.position;
                time = 0;
            }
        }

        time += Time.deltaTime;
    }

    bool loadInGun = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!loadInGun && other.GetComponent<GunController>())
        {
            if (other.GetComponent<GunController>().playerGun)
                StartCoroutine(LoadAmmoInGun(other.GetComponent<GunController>()));
            
            target = WardrobeCollection.Instance.ChekNewTarget();
            AI.destination = target.position;
        }
    }

    private IEnumerator LoadAmmoInGun(GunController gun)
    {
        loadInGun = true;
        while (activeObjCount > 0)
        {
            activeObjCount -= 1;
            gun.LoadAmmo(cartrigeList[activeObjCount].type);
            cartrigeList[activeObjCount].visual.SetActive(false);

            yield return new WaitForSeconds(0.1f);
        }

        loadInGun = false;
    }
}
