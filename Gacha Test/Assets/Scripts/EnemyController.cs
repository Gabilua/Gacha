using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] NavMeshAgent myself;
    [SerializeField] CombatManager combat;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask targetable;
    public Transform target, combatTarget, objectiveTarget;

    [SerializeField] float moveSpeed;
    [SerializeField] float attackRate;
    [SerializeField] float aggroRange, engageDistance, distToTarget;
    [SerializeField] Collider[] baseAtkCols;

    public bool hasObjective;
    bool isMoving, isEngaging, isAggro;
    float attackTimer;

    private void Start()
    {
        myself.speed = moveSpeed;
    }

    void Go()
    {
        myself.isStopped = false;
        myself.SetDestination(target.position);
    }

    void Stop()
    {
        myself.isStopped = true;
    }

    private void Update()
    {
        if (myself.velocity.magnitude > 0)
            isMoving = true;
        else
            isMoving = false;

        anim.SetBool("IsMoving", isMoving);

        combat.healthBar.transform.parent.transform.parent.LookAt(new Vector3(GameManager.instance.cam.transform.position.x, combat.healthBar.transform.parent.transform.parent.position.y, GameManager.instance.cam.transform.position.z));

        if (!combat.isDead)
        {
            TargetSorting();

            Move();
            Combat();
        }
    }

    void TargetSorting()
    {
        if (combatTarget == null)
        {
            if (hasObjective)
                target = objectiveTarget;
            else
                target = null;
        }
        else
            target = combatTarget;
    }

    void AggroDetection()
    {
        if (Physics.CheckSphere(transform.position, aggroRange, targetable))
            isAggro = true;
        else
            isAggro = false;

        if (isAggro)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, aggroRange, targetable);

            if (targets.Length > 0 && targets[0] != null)
                combatTarget = targets[0].transform;
            else
                combatTarget = null;
        }
        else
        {
            if(combatTarget != null)
                combatTarget = null;

            if (combat.healthBar.transform.parent.gameObject.activeInHierarchy)
                combat.healthBar.transform.parent.gameObject.SetActive(false);
        }
    }

    void Combat()
    {
        if (!combatTarget)
            AggroDetection();

        if (isEngaging)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                anim.SetTrigger("Engage");
                attackTimer = attackRate;
            }
        }
    }

    public void EnableBaseAttackCollision(int i)
    {
        baseAtkCols[i].enabled = true;
    }

    public void DisableBaseAttackCollision(int i)
    {
        baseAtkCols[i].enabled = false;
    }

    private void Move()
    {
        if(target != null)
        {
            distToTarget = (transform.position - target.transform.position).magnitude;

            if (isAggro || hasObjective)
            {
                if (distToTarget > engageDistance)
                {
                    Go();

                    isEngaging = false;
                }
                else
                {
                    if (!myself.isStopped)
                    {
                        Stop();

                        isEngaging = true;
                    }
                }
            }
        }             
    }
}
