using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static AmbManager;


public class AmbManager : Amb
{
    

    public float crossFadeDuration = 2.0f;
    
    
    

    [SerializeField] private AmbAudioProfile[] AmbAudioProfiles;
    public AudioMixerGroup outputMixerGroup;



    AudioSource ActiveAS;

    private List<AudioSource> OldAudioSourcesList = new List<AudioSource>();
    private Coroutine FadeCoro;

    [System.Serializable]
    public class AmbAudioProfile
    {
        public AmbTypes AmbTypes;
        public AudioClip AmbSound;
        [Range(0, 1)]
        public float Volume = 1;
    }
    private void Awake()
    {
        ChangeAmbRequest(AmbTypes.Globle);
    }

    public void ChangeAmbRequest(AmbTypes ChangeAmbTo)
    {
        AmbAudioProfile targetProfile = null;
        

        foreach (var ambProfile in AmbAudioProfiles)
        {
            if (ambProfile.AmbTypes == ChangeAmbTo)
            {
                targetProfile = ambProfile;
                break;
            }
        }

        if (targetProfile == null)
            return;

        if (ActiveAS != null)
        {
            if (!OldAudioSourcesList.Contains(ActiveAS))
                OldAudioSourcesList.Add(ActiveAS);
        }

        ActiveAS = null;

     
        for (int i = OldAudioSourcesList.Count - 1; i >= 0; i--)
        {
            AudioSource source = OldAudioSourcesList[i];

   

            if (source.clip == targetProfile.AmbSound)
            {
                ActiveAS = source;
                OldAudioSourcesList.RemoveAt(i);
                break;
            }
        }

        
        if (ActiveAS == null)
        {
            ActiveAS = gameObject.AddComponent<AudioSource>();
            ActiveAS.clip = targetProfile.AmbSound;
            ActiveAS.loop = true;
            ActiveAS.volume = 0f;
            ActiveAS.outputAudioMixerGroup = outputMixerGroup;
            ActiveAS.spatialBlend = 0f;
            ActiveAS.Play();
        }

        

        if (FadeCoro != null)
        {
            StopCoroutine(FadeCoro);
        }


        foreach (var profile in AmbAudioProfiles)
        {
            if (profile.AmbTypes == ChangeAmbTo)
            {
                FadeCoro = StartCoroutine(CrossFade(profile.Volume));
            }
        }
    }
    private IEnumerator CrossFade(float TargetVolume)
    {
        float elapsedTime = 0f;
        float duration = Mathf.Max(0.01f, crossFadeDuration);

        float newStartVolume = ActiveAS.volume;

        
        List<float> oldStartVolumes = new List<float>();

        foreach (var source in OldAudioSourcesList)
        {
            oldStartVolumes.Add(source != null ? source.volume : 0f);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            
            if (ActiveAS != null)
            {
                ActiveAS.volume = Mathf.Lerp(
                    newStartVolume, TargetVolume, t);
            }

            
            for (int i = OldAudioSourcesList.Count - 1; i >= 0; i--)
            {
                AudioSource source = OldAudioSourcesList[i];

                if (source == null)
                {
                    OldAudioSourcesList.RemoveAt(i);
                    continue;
                }

                source.volume = Mathf.Lerp(oldStartVolumes[i], 0f, t);
            }

            yield return null;
        }

        
        for (int i = OldAudioSourcesList.Count - 1; i >= 0; i--)
        {
            if (OldAudioSourcesList[i] != null)
                Destroy(OldAudioSourcesList[i]);

            OldAudioSourcesList.RemoveAt(i);
        }

        if (ActiveAS != null)
            ActiveAS.volume = TargetVolume;
    }

}
