using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler instance;
    [Header("References")]
    [SerializeField] private InputActionAsset playerControls;
    //[SerializeField] private PauseManager pauseManager;
    //[SerializeField] private PauseMenuInputHandler pauseMenuInputHandler;


    [Header("Action Map Name Reference")]
    [SerializeField] private string playerActionMapName = "Player";


    [Header("Player Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "MouseLook";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string openMenu = "OpenMenu";
    [SerializeField] private string interact = "Interact";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction openMenuAction;
    private InputAction interactAction;

    public Vector2 MovementInput {  get; private set; }
    public Vector2 RotationInput {  get; private set; }
    public bool movementTriggered {  get; private set; }
    public bool JumpTriggered {  get; private set; }
    public bool SprintTriggered {  get; private set; }
    public bool Crouchtriggered {  get; private set; }
    public bool MenuTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    [SerializeField] private bool toggleCrouch = false;
    [SerializeField] private bool allowedToCrouch = true;

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(playerActionMapName);


        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);
        crouchAction = mapReference.FindAction(crouch);
        openMenuAction = mapReference.FindAction(openMenu);
        interactAction = mapReference.FindAction(interact);


        SubscribeActionValuesToInputEvents();
    }

    private void Update()
    {
        /*if (crouchAction.canceled && !toggleCrouch)
        {
            Crouchtriggered = false;
        }*/
    }


    public void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;


        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;


        movementAction.performed += inputInfo => movementTriggered = true;
        movementAction.canceled += inputInfo => movementTriggered = false;
        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;


        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;

        crouchAction.performed += inputInfo => Crouchtriggered = true;
        if (toggleCrouch && Crouchtriggered && allowedToCrouch)
        {
            crouchAction.canceled += inputInfo => Crouchtriggered = false;
        }
        else
            crouchAction.canceled += inputInfo => Crouchtriggered = false;
        //crouchAction.canceled += inputInfo => Crouchtriggered = false;

        /*if (!MenuTriggered && !pauseManager.isPaused)
        {
            openMenuAction.performed += inputInfo => MenuTriggered = true;
            //Debug.Log(MenuTriggered);
        }
        if (!MenuTriggered && pauseManager.isPaused)
        {
            //pauseMenuInputHandler.closeMenuAction.performed += inputInfo => MenuTriggered = false;
            //Debug.Log(MenuTriggered);
        }*/

        interactAction.performed += inputInfo => InteractTriggered = true;
        interactAction.canceled += inputInfo => InteractTriggered = false;
    }


    private void OnEnable()
    {
        playerControls.FindActionMap(playerActionMapName).Enable();
    }


    private void OnDisable()
    {
        playerControls.FindActionMap(playerActionMapName).Disable();
    }

}
