using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    HealthUI healthUI;
    public float health = 3;


    public DeathPanelController deathPanelController;

    void Awake()
    {
        healthUI = GameObject.Find("PlayerHUD").GetComponent<HealthUI>();

        if(healthUI == null)
            print("Health UI Missing!");
        
        if(deathPanelController == null)
        {
            deathPanelController = GameObject.Find("LevelManagement").GetComponent<DeathPanelController>();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("lost " + amount + " health");
        Debug.Log("current Health: " + health);
        healthUI.DisplayHealthUI();

        if (health <= 0)
        { /*
            if has player tag
            }
            GameOverTrigger();
            }
            else
            {
            enemyDeath():
            }
            
            */
            GameOverTrigger();

        }

            
    }
    public void GameOverTrigger()
    {
        //player death
        deathPanelController.isDead = true;
    }


}
