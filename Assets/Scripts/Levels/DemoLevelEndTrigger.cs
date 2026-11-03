using System.Collections;
using UnityEngine;

public class DemoLevelEndTrigger : MonoBehaviour
{
    [SerializeField] GameObject youWinText;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerCharacter")
        {
            youWinText.SetActive(true);
            AudioSource temp = GetComponent<AudioSource>();
            temp.Play();
            StartCoroutine(WaitBeforeEndLevel());
        }
    }

    IEnumerator WaitBeforeEndLevel()
    {
        yield return new WaitForSeconds(5f);
        print("Quit");
        Application.Quit();
        yield return null;
    }
}
