using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset systemControls;

    [Header("Action Map Name Rederences")]
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string actionMapName2 = "System";

    [Header("Cation Name Refernces")]
    [SerializeField] private string escape = "Escape";

    [SerializeField] private string move = "Move";
    [SerializeField] private string rotate = "Rotation";
    [SerializeField] private string fire1 = "Fire1";
    [SerializeField] private string fire2 = "Fire2";

    private InputAction escapoeAction;
    private InputAction rotateAction;
    private InputAction moveAction;
    private InputAction fire1Action;
    private InputAction fire2Action;

    public bool EscapeTrigger { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public Vector2 RotateInput { get; private set; }
    public bool Fire1Trigger { get; private set; }
    public bool Fire2Trigger { get; private set; }

    private void Awake()
    {
        #region Singleton

        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        #endregion

        escapoeAction = systemControls.FindActionMap(actionMapName2).FindAction(escape);

        moveAction = systemControls.FindActionMap(actionMapName).FindAction(move);
        rotateAction = systemControls.FindActionMap(actionMapName).FindAction(rotate);
        fire1Action = systemControls.FindActionMap(actionMapName).FindAction(fire1);
        fire2Action = systemControls.FindActionMap(actionMapName).FindAction(fire2);

        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        escapoeAction.performed += ctx => EscapeTrigger = true;
        escapoeAction.canceled += ctx => EscapeTrigger = false;

        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        rotateAction.performed += ctx => RotateInput = ctx.ReadValue<Vector2>();
        rotateAction.canceled += ctx => RotateInput = Vector2.zero;

        fire1Action.performed += ctx => Fire1Trigger = true;
        fire1Action.canceled += ctx => Fire1Trigger = false;

        fire2Action.performed += ctx => Fire2Trigger = true;
        fire2Action.canceled += ctx => Fire2Trigger = false;
    }

    private void OnEnable()
    {
        escapoeAction.Enable();
        moveAction.Enable();
        rotateAction.Enable();
        fire1Action.Enable();
        fire2Action.Enable();
    }

    private void OnDisable()
    {
        escapoeAction.Disable();
        moveAction.Disable();
        rotateAction.Disable();
        fire1Action.Disable();
        fire2Action.Disable();
    }
}