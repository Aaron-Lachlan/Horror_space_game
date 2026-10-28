using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float health = 3;




    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("lost " + amount + " health");
        Debug.Log("current Health: " + health);

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
        }

            
    }
    public void GameOverTrigger()
    {
        //player death

    }
}
