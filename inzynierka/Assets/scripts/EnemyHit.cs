using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    public Enemy enemy;
    private PlayerStats playerStats;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.tag == "Player")
        {

            playerStats = collider.GetComponentInParent<PlayerStats>();
            playerStats.TakeDmg(enemy.dmg);
            Debug.Log("player has ="+playerStats.health);
           
        }

    }
}
