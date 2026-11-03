using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuInputHandler : MonoBehaviour
{
    public static PauseMenuInputHandler instance;
    [Header("References")]
    [SerializeField] private InputActionAsset uIControls;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private PauseManager pauseManager;

    [Header("Action Map Name References")]
    [SerializeField] private string uIActionMapName = "UI";

    [Header("UI Action Name References")]
    [SerializeField] private string closeMenu = "CloseMenu";

    private InputAction closeMenuAction;

    private void Awake()
    {
        InputActionMap mapReference = uIControls.FindActionMap(uIActionMapName);

        closeMenuAction = mapReference.FindAction(closeMenu);

        SubscribeActionValuesToInputEvents();
    }

    public void SubscribeActionValuesToInputEvents()
    {
        if (!playerInputHandler.MenuTriggered && pauseManager.isPaused)
        {
            //closeMenuAction.performed += inputInfo => playerInputHandler.MenuTriggered = false;
            //Debug.Log(MenuTriggered);
        }
    }

    public void OnEnable()
    {
        uIControls.FindActionMap(uIActionMapName).Enable();
    }

    public void OnDisable()
    {
        uIControls.FindActionMap(uIActionMapName).Disable();
    }

}
