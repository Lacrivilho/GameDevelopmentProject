using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;
    public GameObject roomController;

    public float runSpeed = 6f;
    public float aimSpeed = 2f;
    public float gravity = -9.81f * 2;
    
    bool isMoving;
    bool isAiming;
    static bool shootingBlocked;

    private void Awake()
    {
        shootingBlocked = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //get inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        isMoving = x != 0 || z != 0;

        //create movement vector
        Vector3 move = transform.right * x + transform.forward * z;

        if (!controller.isGrounded)
        {
            move.y -= gravity;
        }

        //move player
        if (isAiming)
        {
            controller.Move(move * aimSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * runSpeed * Time.deltaTime);
        }

        animator.SetBool("Walking", isMoving);

        //left mouse shoot
        if (Input.GetKeyDown(KeyCode.Mouse0) && !shootingBlocked)
        {
            animator.SetTrigger("Shoot");
            shootingBlocked = true;
        }
        //right mouse aim
        isAiming = Input.GetKey(KeyCode.Mouse1);
        animator.SetBool("Aiming", isAiming);

        //Check if falling out of world
        if(controller.velocity.y < -12)
        {
            transform.position = roomController.GetComponent<RoomsGenerator>().getCurrentRoom().transform.position;
            UnityEngine.AI.NavMeshHit nearestNavmesh;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out nearestNavmesh, 100, -1))
            {
                transform.position = nearestNavmesh.position;
            }
        }
    }

    public static void unblockShooting()
    {
        shootingBlocked = false;
    }
}
