using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class FootStepsSFX : MaterialType
{
    
    [SerializeField] bool isPlayer;

    [Tooltip("Add different audio for the surfaces so the player can tell its the alien")]
    [SerializeField] bool isAlien;

    [System.Serializable]
    public class SurfaceAudioProfile
    {
        public SurfaceType SurfaceType;
        public AudioClip[] footstepSounds;
        [Range(0, 1)]
        public float VolumeOfAll = 1;
    }
    [Header("Surface Profiles")]
    [SerializeField] private SurfaceAudioProfile[] surfaceProfiles;

    private Dictionary<string, SurfaceAudioProfile> surfaceLookup;
    [SerializeField] AudioSource AudioSource;

    private int LastPlayedClip;
    private int randomIndex;

    void Awake()
    {
        if(AudioSource == null)
        {
            print("Feet not found. Linking...");
            if(isPlayer)
                AudioSource = GameObject.Find("Feet").GetComponent<AudioSource>();
            if(isAlien)
                AudioSource = GameObject.Find("FeetAlien").GetComponent<AudioSource>();
            if(AudioSource != null)
            {
                print("Linked...");
            }
            else
            {
                print("Player must be missing. Add player to level");
            }
        }
    }


    public void PlaySFX(MaterialSfx hitSurfaceType)
    {
        foreach (var profile in surfaceProfiles)
        {
            
            if (profile.SurfaceType == hitSurfaceType.currentSettings)
            {


                while (randomIndex == LastPlayedClip)
                {
                    randomIndex = UnityEngine.Random.Range(0, profile.footstepSounds.Length);
                }

                AudioSource.volume = profile.VolumeOfAll;
                AudioSource.PlayOneShot(profile.footstepSounds[randomIndex]);
                LastPlayedClip = randomIndex;
                return;
                

            }
            
        }   


    }
}
