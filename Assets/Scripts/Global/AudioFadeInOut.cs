using System.Collections;
using UnityEngine;

public class AudioFadeInOut : MonoBehaviour
{
    //[SerializeField] float maxAmbienceVolume;
    public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime) 
    {
        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < 1) 
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public static IEnumerator FadeInListener(float fadeTime) 
    {
        AudioListener.volume = 0f;
        while (AudioListener.volume < 1) 
        {
            AudioListener.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public static IEnumerator FadeInAmbience(AudioSource audioSource, float fadeTime) 
    {
        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < 0.1f) 
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime) 
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0) 
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
    }

    public static IEnumerator FadeOutListener(float fadeTime) 
    {
        float startVolume = AudioListener.volume;
        while (AudioListener.volume > 0) 
        {
            AudioListener.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
