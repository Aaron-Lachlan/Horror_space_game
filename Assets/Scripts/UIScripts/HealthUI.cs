using UnityEngine;

public class HealthUI : MonoBehaviour
{
    Health playerHealthScript;
    [SerializeField] GameObject healthFull;
    [SerializeField] GameObject healthMed;
    [SerializeField] GameObject healthCrit;

    void Awake()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("PlayerCharacter").GetComponent<Health>();
        if(playerHealthScript == null)
        {
            print("Player health is missing!");
        }
        //DisableHealthUI();
        DisplayHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableHealthUI()
    {
        healthFull.SetActive(false);
        healthMed.SetActive(false);
        healthCrit.SetActive(false);
    }

    public void DisplayHealthUI()
    {
        /*if(playerHealthScript.health <= 6.6f)
        {
            healthFull.SetActive(true);
            healthMed.SetActive(false);
            healthCrit.SetActive(false);
        }*/
        if(playerHealthScript.health >= 6.5f || playerHealthScript.health == 10f)
        {
            healthFull.SetActive(true);
            healthMed.SetActive(false);
            healthCrit.SetActive(false);
        }
        else if(playerHealthScript.health >= 3 && playerHealthScript.health <= 6.4f)
        {
            healthFull.SetActive(false);
            healthMed.SetActive(true);
            healthCrit.SetActive(false);
        }
        else if(playerHealthScript.health >= 0 && playerHealthScript.health <= 2.9f)
        {
            healthFull.SetActive(false);
            healthMed.SetActive(false);
            healthCrit.SetActive(true);
        }
    }
}
