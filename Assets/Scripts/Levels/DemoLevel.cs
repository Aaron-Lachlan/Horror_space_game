using UnityEngine;

public class DemoLevel : MonoBehaviour
{
    [SerializeField] Globals globals;
    [SerializeField] bool Testing = true;
    
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

    void Update()
    {
        
    }
}
