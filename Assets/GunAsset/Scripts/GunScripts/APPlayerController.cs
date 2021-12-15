using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APPlayerController : MonoBehaviour
{
    [Header("Player Control")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private GameObject visualPlayer;
    [SerializeField] private Animator anim;

    private Rigidbody _rb;

    [Header("Sounds")]
    [SerializeField] private AudioClip dieAudioClip;
    [SerializeField] private AudioClip useObjAudioClip;

    [Header("Particles")]
    [SerializeField] private ParticleSystem useInteractiveParticle;

    [SerializeField] private Vector3 SpawnPos;

    private bool dieActive = false;

    [SerializeField] private APCameraController _camera;

    [Header("CartrigeIn Setting")]
    [SerializeField] private Transform setPosition;
    [SerializeField] private GameObject visualFireObj;
    private List<CartrigeSetting> cartrigeList = new List<CartrigeSetting>();
    [SerializeField] private float parcerSetObj = 1;
    private int activeObjCount = 0;

    private void Start()
    {
        visualPlayer.SetActive(true);

        dieActive = false;
        SpawnPos = gameObject.transform.position;

        if (GetComponent<Rigidbody>())
            _rb = GetComponent<Rigidbody>();
        else
            Debug.LogError("Not find Rigidbody Component");

        for (int i = 0; i < 20; i++)
        {
            CartrigeSetting setting = new CartrigeSetting();
            setting.visual = Instantiate(visualFireObj, new Vector3(setPosition.position.x, setPosition.position.y + (i * parcerSetObj), setPosition.position.z), setPosition.rotation, setPosition);
            cartrigeList.Add(setting);
            setting.visual.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!GameController.Instance.gameIsPlayed || dieActive)
        {
            _rb.velocity = new Vector3(0, 0, 0);
            return;
        }

        Move();
    }

    public void Move()
    {
        float horizMove = JoystickStickk.Instance.HorizontalAxis();
        float verticalMove = JoystickStickk.Instance.VerticalAxis();

        if (horizMove == 0.0f && verticalMove == 0.0f)
        {
            if (anim.GetBool("Run")) anim.SetBool("Run", false);
            _rb.velocity = new Vector3(0, 0, 0);

            if (!dieActive) _camera.StopMove();
            return;
        }

        if (!dieActive) _camera.StartMove();

        float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;
        visualPlayer.transform.rotation = Quaternion.Euler(0, angle, 0);

        _rb.velocity = new Vector3(horizMove * moveSpeed, 0, verticalMove * moveSpeed);

        if (!anim.GetBool("Run")) anim.SetBool("Run", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dieActive) return;

        if (other.GetComponent<DangerObstacle>())
        {
            StartCoroutine(DieAnim());

            if (SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.ClipPlay(dieAudioClip);
        }

        if (other.GetComponent<APInteractbleObjController>())
        {
            if (SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.ClipPlay(useObjAudioClip);
            anim.SetTrigger("UseItem");

            other.GetComponent<APInteractbleObjController>().UseObject();
        }

        if (!loadInGun && other.GetComponent<GunController>())
        {
            StartCoroutine(LoadAmmoInGun(other.GetComponent<GunController>()));
        }
    }

    bool loadInGun = false;
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

    public void UpgradeCartrigeActiveCount(CartrigeSetting.CartrigeType _type = CartrigeSetting.CartrigeType.mashineGun)
    {
        cartrigeList[activeObjCount].visual.SetActive(true);
        cartrigeList[activeObjCount].type = _type;
        activeObjCount += 1;
    }

    private IEnumerator DieAnim()
    {
        _camera.StopMove();
        dieActive = true;

        anim.SetBool("Run", false);
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(1.75f);

        _camera.StartMove();
        transform.position = SpawnPos;
        visualPlayer.transform.rotation = Quaternion.identity;

        dieActive = false;
    }

    public void WinGame()
    {
        anim.SetBool("Run", false);
        anim.SetBool("WinGame", true);
    }

    public void LoseGame()
    {

        anim.SetBool("Run", false);
        anim.SetBool("LoseGame", true);
    }
}
