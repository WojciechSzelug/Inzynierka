using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    #region Public Variable
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLenght;
    public float attackDistace;
    public float moveSpeed;
    public float timer;
    #endregion

    #region Private Variable
    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    private float intTimer;
    #endregion

    // Update is called once per frame

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (inRange)
        {
           // hit = 
        }
        
    }

    private void OnTriggerEnter2D(Collider2D trig)
    {
        if ( trig.gameObject.tag == "Player"){
            target = trig.gameObject;
            inRange = true;
        }
    }
}
