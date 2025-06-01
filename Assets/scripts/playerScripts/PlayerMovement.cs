using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // this script uses Unity input system to get actions and apply movement
    public InputActionAsset InputActions;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private Vector2 moveAmt;
    private Vector2 lookAmt;
    private Rigidbody rigidbody;

    [Header("Movement Variables")]
    public float walkSpeed = 5;
    public float rotateSpeed = 5;
    public float jumpSpeed = 5;
    // enables player action map from input system
    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    // used to diable when needed
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    // gets all acitons
    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rigidbody = GetComponent<Rigidbody>();
    }
    // takes values from actions such as key press and mouse position
    private void Update()
    {
        moveAmt = moveAction.ReadValue<Vector2>();
        lookAmt = lookAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }
    // jumps by applying force to rigidbody
    public void Jump()
    {
        rigidbody.AddForceAtPosition(new Vector3(0, 5f, 0), Vector3.up, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        Walking();
        Rotating();
    }
    // moves rigidbody based on key press from action map
    public void Walking()
    {
        rigidbody.MovePosition(rigidbody.position + transform.forward * moveAmt.y * walkSpeed * Time.fixedDeltaTime);
        rigidbody.MovePosition(rigidbody.position + transform.right * moveAmt.x * walkSpeed * Time.fixedDeltaTime);
    }
    // rotates rigidbody based on mouse input from action map
    public void Rotating()
    {
        float rotationAmount = lookAmt.x * rotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
        rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
    }
    // initiates crawl position for player
    public void Crawl()
    {

    }
}
