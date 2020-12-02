using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    public EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }



    void OnTriggerExit2D(Collider2D collider)
    {

        if (collider.tag == "Player")
        {
            
            enemyAI.isTriggered = false;
        }

    }

}
