using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{

    public float movespeed;
    public Rigidbody2D rd;

    private Vector2 moveDirection;

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY);
    }

    void Move()
    {
        rd.velocity = new Vector2(moveDirection.x * movespeed, moveDirection.y * movespeed);
    }
}
