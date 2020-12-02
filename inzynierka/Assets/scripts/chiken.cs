using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chiken : MonoBehaviour
{


    public PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.tag == "Player")
        {

            playerStats = collider.GetComponentInParent<PlayerStats>();

            if (playerStats.health<5)
            {
                playerStats.health++;
                Destroy(gameObject);
            }

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
