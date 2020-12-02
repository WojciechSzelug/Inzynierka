using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.tag=="Player")
        {

            
            enemyAI.isTriggered = true;
        }
        
    }




}
