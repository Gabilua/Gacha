using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Here we setup the other components we need;
    [Header("References")]
    [SerializeField] CharacterController controller;
    public CombatManager combat;
    [SerializeField] bl_Joystick joystick;
    [SerializeField] EquipmentManager equipment;
    [SerializeField] Transform center;
    public GameObject GFX;
    public Animator anim;

    // Here we define the values for how the character moves;
    [Header("Character Attributes")]
    public float runSpeed;
    public float turnSpeed, baseSpeed;

    // Here we set variables needed for the controller;
    [Header("Controller Configuration")]
    [SerializeField] float groundLevel;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] string[] playerInput;
    public Collider[] baseAtkCols;
    public float baseAtkCD;
    public GameObject baseAtkProjectile;
    public Transform baseAtkProjectileBarrel;

    [Header("VFX")]
    [SerializeField] ParticleSystem runFX;
    public ParticleSystem[] atkFX;

    [Header("Stats")]
    public Vector3 move;
    public bool isGrounded, canMove, isMoving, isIdle, isAttacking;

    float attackTimer, aggroTimer;
    float x, z, distToGround, dir;
    Vector3 addedForce, nextPos;

    // Here we check if the controller's bottom is touching valid Ground;
    void GroundCheck()
    {
        //Touched Ground
        if (Physics.Raycast(center.transform.position, -transform.up, groundLevel, groundLayer) && !isGrounded)
            move.y = 0;

        RaycastHit hit;
        if (Physics.Raycast(center.transform.position, -transform.up, out hit, groundLevel, groundLayer))
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && isMoving)
        {
            if (!runFX.isPlaying)
                runFX.Play();
        }
        else
            runFX.Stop();

        anim.SetBool("IsGrounded", isGrounded);
    }

    // Here we make the character always face the direction of its movement;
    void Orientation()
    {
        nextPos = new Vector3(x, 0, z);

        if (canMove)
        {
            if (x == 0 && z == 0)
                return;
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos), turnSpeed * Time.deltaTime);
        }
    }

    // Here we pass horizontal input and constant speed to the controller's movement, as well as adding fake gravity;
    void Movement()
    {
        anim.SetBool("IsMoving", isMoving);

        if (z != 0 && x != 0)
            move = new Vector3((x * runSpeed * .6f) + addedForce.x, move.y, (z * runSpeed * .6f) + addedForce.z);
        else
            move = new Vector3((x * runSpeed) + addedForce.x, move.y, (z * runSpeed) + addedForce.z);

        if (!isGrounded)
            move.y = (Mathf.Lerp(move.y, -30, 3 * Time.deltaTime));

        controller.Move(move * Time.deltaTime);

        if (canMove)
        {
            if (x != 0 || z != 0)
                isMoving = true;
            else
                isMoving = false;
        }
        else
            isMoving = false;
    }

    // Here we fake physics forces;
    public void ApplyForce(Vector3 dir)
    {
        addedForce = dir;
        move.y = addedForce.y;
    }

    // Here we gradually damp the forces inside the fake force vector to 0;
    void ForceDamp()
    {
        if (addedForce.x != 0)
        {
            if (addedForce.x < 0)
                addedForce.x = Mathf.Lerp(addedForce.x, 0.0001f, 5 * Time.deltaTime);
            else if (addedForce.x > 0)
                addedForce.x = Mathf.Lerp(addedForce.x, -0.0001f, 5 * Time.deltaTime);
        }

        if (addedForce.y != 0)
        {
            if (addedForce.y < 0)
                addedForce.y = Mathf.Lerp(addedForce.y, 0.0001f, 5 * Time.deltaTime);
            else if (addedForce.y > 0)
                addedForce.y = Mathf.Lerp(addedForce.y, -0.0001f, 5 * Time.deltaTime);
        }
    }

    // Here we define what stops movement;
    void MovementAllower()
    {
        if (isAttacking)
            canMove = false;
        else
            canMove = true;

        if(canMove)
        {
            if (runSpeed < baseSpeed)
                runSpeed = Mathf.Lerp(runSpeed, baseSpeed, 10 * Time.deltaTime);
        }
        else
        {
            if (runSpeed > 0)
                runSpeed = Mathf.Lerp(runSpeed, 0, 10 * Time.deltaTime);
        }
    }

    // Here we check for input;
    void InputCheck()
    {
        x = Input.GetAxis(playerInput[0]) + (joystick.Horizontal / 5);
        z = Input.GetAxis(playerInput[1]) + (joystick.Vertical / 5);

        anim.SetBool("IsIdle", isIdle);

        if (!anim.GetCurrentAnimatorStateInfo(1).IsName("None") && !anim.IsInTransition(1))
            isAttacking = true;
        else
            isAttacking = false;
    }

    public void BasicAttack()
    {
        equipment.Unsheath();

        aggroTimer = 5;

        if (anim.GetCurrentAnimatorStateInfo(1).IsName("None") && attackTimer <= 0)
        {
            anim.SetTrigger("First");
            attackTimer = baseAtkCD;
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 01"))
            anim.SetTrigger("Second");
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 02"))
            anim.SetTrigger("Third");
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 03"))
        {
            anim.ResetTrigger("First");
            anim.ResetTrigger("Second");
            anim.ResetTrigger("Third");
        }
    }

    public void AttackFX(int i)
    {
        atkFX[i].Play();
    }

    public void ShootBaseAtkProjectile(int i)
    {
        GameObject shot = Instantiate(baseAtkProjectile, baseAtkProjectileBarrel.position, baseAtkProjectileBarrel.rotation);
        shot.GetComponent<Projectile>().damage = GetComponent<CombatManager>().baseAtkDmg[i];
        shot.GetComponent<Projectile>().player = combat;

        shot.GetComponent<Rigidbody>().AddForce(transform.forward * shot.GetComponent<Projectile>().speed, ForceMode.Impulse);
        shot.transform.forward = transform.forward;
    }

    public void EnableBaseAttackCollision(int i)
    {
        baseAtkCols[i].enabled = true;
    }

    public void DisableBaseAttackCollision(int i)
    {
        baseAtkCols[i].enabled = false;
    }

    public void Spawn(Transform spawnPoint)
    {
        controller.enabled = false;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        controller.enabled = true;
    }

    private void Update()
    {
        if (aggroTimer > 0)
        {
            aggroTimer -= Time.deltaTime;

            if (isIdle)
                isIdle = false;
        }
        else
        {
            if (!isIdle)
                isIdle = true;
        }

        MovementAllower();

        InputCheck();
        GroundCheck();

        Movement();
        Orientation();
        ForceDamp();

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Trigger")
        {
            if (other.name == "Missions")
                UIManager.instance.ToggleMissionsTab(true);
            else if (other.name == "Exit" && GameManager.instance.missionSuccess)
                UIManager.instance.ToggleProgressCheckScreen(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Trigger")
        {
            if (other.name == "Missions")
                UIManager.instance.ToggleMissionsTab(false);
        }
    }
}
