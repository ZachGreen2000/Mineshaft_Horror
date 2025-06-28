using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Splines;

public class PlayerMovement : MonoBehaviour
{
    // this script uses Unity input system to get actions and apply movement
    public InputActionAsset InputActions;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    public InputAction interactAction;

    private Vector2 moveAmt;
    private Vector2 lookAmt;
    private Rigidbody rigidbody;
    private CapsuleCollider collider;

    //movement variables for the crouching position
    private float ColliderSizeTarget = 0.85f;
    private float ColliderPosTarget = -0.52f;
    private float crawlHeight = 0.987f;

    [Header("Animation")]
    public Animator anim;

    [Header("Movement Variables")]
    public float walkSpeed = 5;
    public float rotateSpeed = 5;
    public float jumpSpeed = 5;
    public bool isCrawling = false;
    public float crouchingSpeed;
    public float crawlingSpeed = 2;
    public bool isOnSpline = false;
    public bool canWalk = true;
    private float splinePos = 0f;

    [Header("Camera Variables")]
    public float targetFov;
    public float fovSpeed;
    private float cameraPitch = 0f;
    private float baseCrawlPitch = 0f;
    public float crawlPitchLimit = 15f;


    [Header("World Objects")]
    public Camera cam;
    public SplineContainer spline1;

    public static PlayerMovement Instance;
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
        Instance = this;

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        interactAction = InputSystem.actions.FindAction("Interact");

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        cameraPitch = cam.transform.localEulerAngles.x;
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

        if (isOnSpline)
        {
            caveCrawlNavigate();
        }

        HandleCameraRotation();
    }
    // jumps by applying force to rigidbody
    public void Jump()
    {
        rigidbody.AddForceAtPosition(new Vector3(0, 5f, 0), Vector3.up, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (canWalk)
        {
            Walking();
            Rotating();
            // movement detection for animation
            if (rigidbody.linearVelocity.magnitude > 0)
            {
                anim.SetBool("isWalking", true);
            }else
            {
                anim.SetBool("isWalking", false);
            }
        }
        
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
        isCrawling = true;
        Vector3 camPos = cam.transform.localPosition;
        Vector3 crawlCamPos = new Vector3(camPos.x, camPos.y - crawlHeight, camPos.z);
        StartCoroutine(SmoothCrawl(crawlCamPos));
        isOnSpline = true;
        canWalk = false;
        Debug.Log("Cam position" + cam.transform.localPosition);

        collider.height = ColliderSizeTarget;
        collider.center = new Vector3(collider.center.x, ColliderPosTarget, collider.center.z);

        baseCrawlPitch = cam.transform.localEulerAngles.x;
        if (baseCrawlPitch > 180f) baseCrawlPitch -= 360f; // Convert to signed angle


    }
    // process smooth crawl from standing the crouch
    IEnumerator SmoothCrawl(Vector3 crawlTarget)
    {
        while (Vector3.Distance(cam.transform.localPosition, crawlTarget) > 0.01f)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, crawlTarget, crouchingSpeed * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, fovSpeed * Time.deltaTime);
            yield return null;
        }
    }
    // this function controls the movement along the spline for the cave crawling
    public void caveCrawlNavigate()
    {
        splinePos += moveAmt.y * crawlingSpeed * Time.deltaTime;
        splinePos = Mathf.Clamp01(splinePos);

        // takes splines position and rotation to determine player movement
        var spline = spline1.Spline;
        Vector3 position = spline.EvaluatePosition(splinePos);
        Vector3 tangent = spline.EvaluateTangent(splinePos);
        Vector3 finalPosition = new Vector3(position.x - 4, position.y + 1, position.z);

        this.transform.position = finalPosition;
        this.transform.rotation = Quaternion.LookRotation(tangent);
    }
    
    private void HandleCameraRotation()
    {
        float pitchInput = lookAmt.y * rotateSpeed * Time.deltaTime;
        cameraPitch -= pitchInput;

        if (isCrawling)
        {
            float minPitch = baseCrawlPitch - crawlPitchLimit;
            float maxPitch = baseCrawlPitch + crawlPitchLimit;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
        }

        Vector3 camEuler = cam.transform.localEulerAngles;
        cam.transform.localEulerAngles = new Vector3(cameraPitch, camEuler.y, 0);
    }

}
