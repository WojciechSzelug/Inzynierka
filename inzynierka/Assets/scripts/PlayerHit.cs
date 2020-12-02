using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{

    private Enemy enemy;
    private PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            
            enemy = other.GetComponentInParent < Enemy > ();
            enemy.takedmg(playerStats.dmg);
            Debug.Log(enemy.hp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
