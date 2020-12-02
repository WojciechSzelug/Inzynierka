using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovment : MonoBehaviour
{

    public float movespeed;
    public Rigidbody2D rd;
    public Animator animator;
    public float attackRate = 0.5f;

    private float nextAttackTime = 0f;
    private Vector2 moveDirection;

    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Rotation();
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

       if (Time.time >= nextAttackTime)
        {

            if (Input.GetAxisRaw("Fire1") != 0)
            {
                animator.SetTrigger("atack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
       }
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0); 
        }

        moveDirection = new Vector2(moveX, moveY);

        animator.SetFloat("Speed", (Mathf.Abs(moveX) + Mathf.Abs(moveY)));
        
        
    }

    void Rotation()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = new Vector2(
        mousePosition.x - transform.position.x,
        mousePosition.y - transform.position.y
        );

        transform.up = direction;

    }
    void Move()
    {

        transform.position = (Vector2)transform.position + new Vector2(moveDirection.x * movespeed, moveDirection.y * movespeed) * Time.deltaTime; ;
        
        
        //rd.velocity = new Vector2(moveDirection.x * movespeed, moveDirection.y * movespeed)*Time.deltaTime;
    }
}
