using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyAI : MonoBehaviour
{

    public Transform target;

    public float speed ;
    public float nextWaypointDistance = 0.2f;
    public Animator animator;
    public bool isTriggered;



    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
   
    
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        

        InvokeRepeating("UpdatePath", 0f, .5f);
        
    }

    void UpdatePath()
    {
        if (seeker.IsDone())      
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }


    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;

        reachedEndOfPath = false;

        float distanceToWaypoint;
        if (isTriggered)
        {
            while (true)
            {
                // If you want maximum performance you can check the squared distance instead to get rid of a
                // square root calculation. But that is outside the scope of this tutorial.
                distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    // Check if there is another waypoint or if we have reached the end of the path
                    if (currentWaypoint + 1 < path.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        // Set a status variable to indicate that the agent has reached the end of the path.
                        // You can use this to trigger some special code if your game requires that.
                        reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            //var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

            float distance = Vector2.Distance((Vector2)transform.position, path.vectorPath[currentWaypoint]);

            if (!(distance < nextWaypointDistance))
            {



                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
                Vector2 force = direction * speed * Time.deltaTime;

                Vector2 lookAt = ((Vector2)target.position - (Vector2)transform.position);

                float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = angle;

                transform.position = (Vector2)transform.position + force;

                animator.SetBool("canWalk", true);

                  
            }
            else
            {
                animator.SetBool("canWalk", false);
            }
        }
        else
        {
            animator.SetBool("canWalk", false);
        }
       

        //Vector2 move = direction * 5 * Time.deltaTime;

        //transform.position = Vector2.MoveTowards(transform.position, direction, 4 * Time.deltaTime);



    }


}
