using UnityEngine;

public class HealthStation : MonoBehaviour
{
    Health playerHealth;
    HealthUI healthUI;

    bool playerUsedHealingStation;

    void Awake()
    {
        playerHealth = GameObject.FindWithTag("PlayerCharacter").GetComponent<Health>();
        if(playerHealth == null)
            print("Health missing");
        
        healthUI = GameObject.Find("PlayerHUD").GetComponent<HealthUI>();

        if(healthUI == null)
            print("Health UI Missing!");
    }

    public void HealPlayer()
    {
        if(playerHealth.health < 10 && !playerUsedHealingStation)
        {
            playerHealth.health = 10;
            playerUsedHealingStation = true;
            healthUI.DisplayHealthUI();
            print("Player healed");
        }
        else
        {
            print("Player in good health");
        }
    }
}
