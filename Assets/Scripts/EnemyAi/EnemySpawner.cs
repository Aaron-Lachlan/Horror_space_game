using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab, spawnerTrigger;

    public Transform spawnTransform;

    public AudioSource  alienSpawnSound;

    public FootStepsSFX alienFootStepsSFX;

    public GameObject alienFootStepsSFXObject;


    private void Awake()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerCharacter"))
        {
            SpawnEnemy();
            
        }
    }


    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnTransform.position, Quaternion.identity);

        alienSpawnSound.Play();

        StartCoroutine(TurnOffTrigger());

    }
    private IEnumerator TurnOffTrigger()
    {
        alienFootStepsSFX.AudioSource = GameObject.Find("FeetAlien").GetComponent<AudioSource>();

        alienFootStepsSFXObject.GetComponent<FootStepsSFX>().enabled = true;

        yield return new WaitForSeconds(0.5f);
        spawnerTrigger.SetActive(false);
    }
   
}
