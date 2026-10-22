using System.Collections;

using UnityEngine;
using UnityEngine.Audio;


public class AmbManager : Amb
{
    [System.Serializable]
    public class AmbAudioProfile
    {
        public AmbTypes AmbTypes;
        public AudioClip AmbSound;
        [Range(0, 1)]
        public float Volume = 1;
    }
    public float crossFadeDuration = 2.0f;
    
    private Coroutine fadeCoroutine;

    [SerializeField] private AmbAudioProfile[] AmbAudioProfiles;
    public AudioMixerGroup outputMixerGroup;

    AudioSource NewAS;
    AudioSource OldAS;

    private void Awake()
    {
        OldAS = gameObject.AddComponent<AudioSource>();//Fake Audio source for 1st startup
        ChangeAmbRequest(AmbTypes.Globle);

    }

    public void ChangeAmbRequest(AmbTypes ChangeAmbTo)
    {

        
            //has bug when entering and exiting a zone before crossfade is compleated


            NewAS = gameObject.AddComponent<AudioSource>();
            NewAS.volume = 0;
            NewAS.loop = true;
            NewAS.outputAudioMixerGroup = outputMixerGroup;
            NewAS.spatialBlend = 0f;

        

       

        foreach (var profile in AmbAudioProfiles)
        {
            if (profile.AmbTypes == ChangeAmbTo)
            {

                NewAS.clip = profile.AmbSound;
                NewAS.Play();
                StartCoroutine(CrossFade(profile.Volume));

                
                
                return;


            }

        }
    }
    private IEnumerator CrossFade(float TargetVolume)
    {
        float elapsedTime = 0f;

        float oldStartVolume = OldAS.volume;
        float newStartVolume = NewAS.volume;

        while (elapsedTime <= crossFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / crossFadeDuration;
            OldAS.volume = Mathf.Lerp(oldStartVolume, 0f, t);
            NewAS.volume = Mathf.Lerp(newStartVolume, TargetVolume, t);

            yield return null;
        }
        

        Destroy(OldAS);
        OldAS = null;

        OldAS = NewAS;
        NewAS = null;
        
        yield return null;
    }

}
