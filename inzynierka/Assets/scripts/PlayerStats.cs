using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour { 

    public int maxHealth = 5;
    public int dmg = 1;
    public int coins;
    public int health; 
    public Image[] hearts; 

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDmg(int dmg)
    {
        health -= dmg;
        if (health == 0) end();
      
    }
    public void end()
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
