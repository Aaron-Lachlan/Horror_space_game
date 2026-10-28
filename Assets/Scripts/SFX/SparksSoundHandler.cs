using UnityEngine;

public class SparksSoundHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem sparksParticleSystem;
    //particle

    [Space]
    [SerializeField] AudioClip[] sparkClips;
    [SerializeField] AudioSource sparksSource;

    bool canplaySoundAgain;

    void Update()
    {
        //sparksSource.PlayOneShot(sparkClips[Random.Range(0, sparkClips.Length)]);
        int currentNumberOfParticles = sparksParticleSystem.particleCount;
        if(currentNumberOfParticles >= 2 && canplaySoundAgain)
        {
            canplaySoundAgain = false;
            sparksSource.PlayOneShot(sparkClips[Random.Range(0, sparkClips.Length)]);
        }

        if(currentNumberOfParticles == 0)
        {
            canplaySoundAgain = true;
        }
    }
}
