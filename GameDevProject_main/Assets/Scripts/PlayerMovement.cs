using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public Animator animator;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    
    bool isMoving;
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

        if(!controller.isGrounded)
        {
            move.y -= gravity;
        }

        //move player
        controller.Move(move * speed * Time.deltaTime);

        animator.SetBool("Walking", isMoving);

        //left mouse shoot
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Shoot");
        }
    }
}
