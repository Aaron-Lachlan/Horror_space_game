using UnityEngine;

public class FootStepPlayer : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] bool isAlien;
    [SerializeField] FootStepsSFX FootStepsSFX;


    public float TimeTillNextStep = 1;
    float StepCycleTimer = 0;
    [SerializeField] float rayLength = 0.5f;

    Vector3 lastPosition;
    Vector3 Velocity;
    private void Start()
    {
        
        if(FootStepsSFX == null)
        {
            if(isPlayer)
                FootStepsSFX = GameObject.FindWithTag("PlayerStepSFX").GetComponent<FootStepsSFX>();
            
            if(isAlien)
                FootStepsSFX = GameObject.FindWithTag("AlienStepSFX").GetComponent<FootStepsSFX>();
        }
    }
    private void Update()
    {
        GroundCheck();

        Velocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;

        

    }
    #region Footsteps
    public void GroundCheck()
    {
       
        

        if (Velocity.magnitude > 0.5)
        {


            StepCycleTimer += Velocity.magnitude * Time.deltaTime;

        }
        if (StepCycleTimer >= TimeTillNextStep && Physics.Raycast(transform.position, Vector3.down, out RaycastHit Hit, rayLength))
        {
            
            MaterialSfx SurfaceType = Hit.collider.GetComponent<MaterialSfx>();
            if (SurfaceType != null)
            {
                
                FootStepsSFX.PlaySFX(SurfaceType);
                
            }

            StepCycleTimer = 0;
        }

    }
    #endregion
}
