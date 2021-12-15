using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class APWarriorController : MonoBehaviour
{
    public NavMeshAgent AI;

    [Header("Move Setting")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float newMovePointTime = 5;
    private Vector3 startPos;
    [SerializeField] private float newMovePointRadius = 1;

    [Header("Attack Setting")]
    [SerializeField] private float AttackDamage = 50;
    [SerializeField] private float reloadTime = 5;

    [Header("")]
    [SerializeField] private float healPoint = 50;
    [SerializeField] private ParticleSystem dieParticle; 

    public Animator anim; 

    private Vector3 target;

    [HideInInspector] public bool attackMode = false;

    private void Start()
    {
        startPos = transform.position;

        AI = GetComponent<NavMeshAgent>();
        AI.speed = 0;
        AI.destination = transform.position;
        attackMode = false;
    }

    private void FixedUpdate()
    {
        if (healPoint <= 0) return;

        if (!attackMode && (AI.velocity.x != 0 && AI.velocity.z != 0))
            anim.SetBool("Run", true);
        else
            anim.SetBool("Run", false);
    }

    private IEnumerator GetNewPoint()
    {
        yield return new WaitForSeconds(newMovePointTime);

        target = new Vector3(startPos.x + Random.Range(-newMovePointRadius, newMovePointRadius), startPos.y, startPos.z + Random.Range(-newMovePointRadius, newMovePointRadius));
        RaycastHit hit; 
        Physics.Raycast(transform.position, target - transform.position, out hit);

        if (hit.collider == null)
            AI.destination = target;
        else
            AI.destination = hit.point;

        AI.speed = moveSpeed;
    }
}
