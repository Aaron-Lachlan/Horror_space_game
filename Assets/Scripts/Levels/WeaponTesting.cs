using UnityEngine;

public class WeaponTesting : MonoBehaviour
{
    [SerializeField] Globals globals;
    [SerializeField] bool Testing = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Testing)
        {
            GameObject globalsObject = new GameObject("GlobalObject");
            globalsObject.AddComponent<Globals>();
            globals = GameObject.Find("GlobalObject").GetComponent<Globals>();
            Debug.Log(globals);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
