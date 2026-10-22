using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] CanvasGroup gameplayCanvasGroup;
    [SerializeField] CanvasGroup skipCanvasGroup;
    public bool fadeIn = false;
    public bool fadeOut = false;

    [Tooltip("Lower = Slower")]
    public float gameplayTimeToFade = 2f;
    public float skiptimeToFade = 0.4f;

    public bool gameplayFade;
    //[SerializeField] bool cinematicFade;
    public bool skipFade;
    public bool instantFade = false;
    
    void Update()
    {
        if(gameplayCanvasGroup == null || skipCanvasGroup == null)
        {
            print("Fade canvas not found. Set it in inspect. Finding now...");
            gameplayCanvasGroup = GameObject.Find("FadeImageGameplay").GetComponent<CanvasGroup>();
            //canvasGroup[1] = GameObject.Find("FadeImageCinematic").GetComponent<CanvasGroup>();
            skipCanvasGroup = GameObject.Find("FadeImageSkip").GetComponent<CanvasGroup>();
        }

        if(fadeIn == true)
        {
            if(gameplayFade)
            {
                if(gameplayCanvasGroup.alpha < 1)
                {
                    if(!instantFade)
                        gameplayCanvasGroup.alpha += gameplayTimeToFade * Time.deltaTime;
                    else
                        gameplayCanvasGroup.alpha = 1;
                    if(gameplayCanvasGroup.alpha >= 1)
                    {
                        fadeIn = false;
                        gameplayFade = false;
                    }
                }
            }

            if(instantFade)
                    gameplayCanvasGroup.alpha = 1;

            if(skipFade)
            {
                if(skipCanvasGroup.alpha < 1)
                {
                    if(!instantFade)
                        skipCanvasGroup.alpha += skiptimeToFade * Time.deltaTime;
                    else
                        skipCanvasGroup.alpha = 1;
                    if(skipCanvasGroup.alpha >= 1)
                    {
                        fadeIn = false;
                        skipFade = false;
                    }
                }
            }
        }

        if(fadeOut == true)
        {
            if(gameplayFade)
            {
                if(gameplayCanvasGroup.alpha >= 0)
                {
                    if(!instantFade)
                        gameplayCanvasGroup.alpha -= gameplayTimeToFade * Time.deltaTime;
                    else
                        gameplayCanvasGroup.alpha = 0; 
                    if(gameplayCanvasGroup.alpha == 0)
                    {
                        fadeOut = false;
                        skipFade = false;
                    }
                }
            }

            if(skipFade)
            {
                if(skipCanvasGroup.alpha >= 0)
                {
                    if(!instantFade)
                        skipCanvasGroup.alpha -= skiptimeToFade * Time.deltaTime;
                    else
                        skipCanvasGroup.alpha = 0;
                    if(skipCanvasGroup.alpha == 0)
                    {
                        fadeOut = false;
                        skipFade = false;
                    }
                }
            }
        }
    }

    //Make Black Screen Appear
    public void FadeIn()
    {
        fadeIn = true;
        //Fade();
    }

    //Make Black Screen Disappear
    public void FadeOut()
    {
        fadeOut = true;
        //Fade();
    }

    /*void Fade()
    {
        if(fadeIn == true)
        {
            if(gameplayFade)
            {
                if(canvasGroup[0].alpha < 1)
                {
                    canvasGroup[0].alpha += gameplayTimeToFade * Time.deltaTime;
                    if(canvasGroup[0].alpha >= 1)
                    {
                        fadeIn = false;
                    }
                }
            }

            if(skipFade)
            {
                if(canvasGroup[1].alpha < 1)
                {
                    canvasGroup[1].alpha += skiptimeToFade * Time.deltaTime;
                    if(canvasGroup[1].alpha >= 1)
                    {
                        fadeIn = false;
                    }
                }
            }
        }

        if(fadeOut == true)
        {
            if(gameplayFade)
            {
                if(canvasGroup[0].alpha >= 0)
                {
                    canvasGroup[0].alpha -= gameplayTimeToFade * Time.deltaTime;
                    if(canvasGroup[0].alpha == 0)
                    {
                        fadeOut = false;
                    }
                }
            }

            if(skipFade)
            {
                if(canvasGroup[1].alpha >= 0)
                {
                    canvasGroup[1].alpha -= skiptimeToFade * Time.deltaTime;
                    if(canvasGroup[1].alpha == 0)
                    {
                        fadeOut = false;
                    }
                }
            }
        }
    }*/

    public IEnumerator WaitTillFaded()
    {
        print("Waiting to be faded");

        while (fadeIn)
        {
            yield return null;
        }

        print("faded"); 
    }
}
