using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int hp = 5;
    public int dmg = 1;
    public BoardManager boardManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void dead()
    {
        Destroy(gameObject);
        boardManager = GameObject.Find("Board").GetComponent<BoardManager>();
        boardManager.NumberOfEnemy--;
        boardManager.EnemyLeft();

        Debug.Log("enemy destroyed");
    }

    public void takedmg(int dmg)
    {
        hp -= dmg;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            dead();
        }
    }
}
