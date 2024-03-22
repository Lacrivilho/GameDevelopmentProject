using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;

    Vector3 velocity;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //get inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //create movement vector
        Vector3 move = transform.right * x + transform.forward * z;

        if(!controller.isGrounded)
        {
            move.y -= gravity;
        }

        //move player
        controller.Move(move * speed * Time.deltaTime);

        if(lastPosition != gameObject.transform.position && controller.isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        lastPosition = gameObject.transform.position;
    }
}
