using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{


    [SerializeField] SoundArray SoundArray;

    [SerializeField] float RandomTime;
    [Range(0, 100)]
    [SerializeField] float RandomTimeMax;
    [Range(0, -100)]
    [SerializeField] float RandomTimeMin;


    float Timer = 0;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //need for first setting of Timer
        Timer = RandomTime + Random.Range(RandomTimeMax, RandomTimeMin);
    }
    void Update()
    {
        

        Timer -= 1 * Time.deltaTime;
        if (Timer <= 0)
        {
            pickAndPlay();
            Timer = RandomTime + Random.Range(RandomTimeMax, RandomTimeMin);
           
        }


    }
    private void pickAndPlay()
    {
        
        audioSource.PlayOneShot(SoundArray.sounds[Random.Range(0, SoundArray.sounds.Length)]);
    }
}
