using UnityEngine;

public class OpenFuseBox : MonoBehaviour
{

    public bool canOpenDoor;

    public GameObject fuseBoxTrigger;

    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canOpenDoor && Input.GetKeyDown(KeyCode.E)) 
        {
            
            OpenPanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCharacter"))
        {
            canOpenDoor = true;


        }
    }

    private void OpenPanel()
    {
        animator.SetTrigger("OpenTrigger");

        fuseBoxTrigger.GetComponent<BoxCollider>().enabled = true;
    }


}
