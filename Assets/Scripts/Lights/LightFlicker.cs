using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light targetLight;

    public float minIntensity = 0.1f;
    public float maxIntensity = 2f;
    [Tooltip("cannot be changed during runtime")]
    public float flickerSpeed = 0.1f;
    public float smoothness = 5f;

    private float targetIntensity;

    void Start()
    {
        
        targetLight = GetComponent<Light>();

        targetIntensity = targetLight.intensity;
        InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
    }

    void Flicker()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void Update()
    {
        targetLight.intensity = Mathf.Lerp(
            targetLight.intensity,
            targetIntensity,
            smoothness * Time.deltaTime
        );
    }
}
