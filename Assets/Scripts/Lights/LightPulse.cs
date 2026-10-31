using UnityEngine;

public class LightPulse : MonoBehaviour
{
    Light Light;
    public float amplitude;
    public float frequncy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        Light.intensity = amplitude * Mathf.Sin(Time.time * frequncy);
    }
}
