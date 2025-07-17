using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Splines;
using System.Linq;

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
    private float armCrawlHeight = 1.2f;
    private bool canCrawlStep;

    [Header("Animation")]
    public Animator anim;

    [Header("Movement Variables")]
    public float walkSpeed = 5;
    public float rotateSpeed = 5;
    public float jumpSpeed = 5;
    public bool isCrawling = false;
    public float crouchingDuration;
    public float crawlingSpeed = 2;
    public bool isOnSpline = false;
    public bool canWalk = true;
    private float splinePos = 0f;

    [Header("Camera Variables")]
    public float targetFov;
    public float fovSpeed;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;
    private float baseCrawlPitch = 0f;
    private float baseCrawlYaw = 0f;
    private float crawlPitchLimit = 15f;
    private float crawlYawLimit = 15f;


    [Header("World Objects")]
    public Camera cam;
    public SplineContainer spline1;
    public GameObject playerArms;

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
        cameraYaw = cam.transform.localEulerAngles.y;
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
        // run grab animation
        if (interactAction.WasPressedThisFrame() && !anim.GetBool("isGrabbing"))
        {
            anim.SetBool("isGrabbing", true);
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
        }
        
    }

    // moves rigidbody based on key press from action map
    public void Walking()
    {
        rigidbody.MovePosition(rigidbody.position + transform.forward * moveAmt.y * walkSpeed * Time.fixedDeltaTime);
        rigidbody.MovePosition(rigidbody.position + transform.right * moveAmt.x * walkSpeed * Time.fixedDeltaTime);
        // movement detection for animation, runs candle hold animation if player items list contains candle
        if (moveAmt != new Vector2(0,0) && !GameManager.Instance.checkItems("candle"))
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("hasCandle", false);
        }
        else if (GameManager.Instance.checkItems("candle"))
        {
            anim.SetBool("hasCandle", true);
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("hasCandle", false);
            anim.SetBool("isWalking", false);
        }
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
        Vector3 armPos = playerArms.transform.localPosition;
        Vector3 crawlArmPos = new Vector3(armPos.x, armPos.y - armCrawlHeight, armPos.z);
        BezierKnot knot0Local = spline1.Spline.Knots.ElementAt(0);
        Vector3 knot0World = spline1.transform.TransformPoint(knot0Local.Position);
        Vector3 targetKnot0Pos = new Vector3(knot0World.x, knot0World.y + 0.4f, knot0World.z); // adding offset
        StartCoroutine(SmoothCrawl(crawlCamPos, crawlArmPos, targetKnot0Pos));
        isOnSpline = true;
        canCrawlStep = true;
        canWalk = false;
        Debug.Log("Cam position" + cam.transform.localPosition);

        collider.height = ColliderSizeTarget;
        collider.center = new Vector3(collider.center.x, ColliderPosTarget, collider.center.z);

        baseCrawlPitch = cam.transform.localEulerAngles.x;
        baseCrawlYaw = cam.transform.localEulerAngles.y;
        if (baseCrawlPitch > 180f) baseCrawlPitch -= 360f; // Convert to signed angle
        if (baseCrawlYaw > 180f) baseCrawlYaw -= 360f;
    }

    // process smooth crawl from standing the crouch
    IEnumerator SmoothCrawl(Vector3 crawlTarget, Vector3 crawlArmPos, Vector3 knot0Spline)
    {
        anim.SetBool("crawlTransition", true);
        float elapsed = 0f;
        float duration = crouchingDuration;

        Vector3 startCamPos = cam.transform.localPosition;
        Vector3 startArmPos = playerArms.transform.localPosition;
        Vector3 startPlayerPos = this.transform.position;
        float startFov = cam.fieldOfView;

        while (elapsed < duration) // (Vector3.Distance(cam.transform.localPosition, crawlTarget) > 0.05f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            cam.transform.localPosition = Vector3.Lerp(startCamPos, crawlTarget, t);
            playerArms.transform.localPosition = Vector3.Lerp(startArmPos, crawlArmPos, t);
            this.transform.position = Vector3.Lerp(startPlayerPos, knot0Spline, t);
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, t);
            yield return null;
        }
        anim.SetBool("crawlTransition", false);
        anim.SetBool("isCrawling", true);
        // rotate arms slightly
        playerArms.transform.localRotation = Quaternion.Euler(
            playerArms.transform.localEulerAngles.x,
            playerArms.transform.localEulerAngles.y + -10f,
            playerArms.transform.localEulerAngles.z
        );
        Debug.Log("SmoothCrawl completed");
        cam.transform.localPosition = crawlTarget;
        playerArms.transform.localPosition = crawlArmPos;
        this.transform.position = knot0Spline;
    }

    // this function controls the movement along the spline for the cave crawling
    public void caveCrawlNavigate()
    {
        // running crawling animation
        if (moveAction.WasPressedThisFrame() && canCrawlStep)
        {
            anim.SetTrigger("crawlTrigger");
            StartCoroutine(crawlDistance());
        }
    }

    // this coroutine takes the animation length of the crawl and moves the player proportionally
    IEnumerator crawlDistance()
    {
        canCrawlStep = false;

        float elapsed = 0f;
        float animationDuration = 1f;
        float startSplinePos = splinePos;
        float targetSplinePos = Mathf.Clamp01(startSplinePos + 0.05f);

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            splinePos = Mathf.Lerp(startSplinePos, targetSplinePos, t);

            // takes splines position and rotation to determine player movement
            var spline = spline1.Spline;
            Vector3 position = spline.EvaluatePosition(splinePos);
            Vector3 tangent = spline.EvaluateTangent(splinePos);
            Vector3 finalPosition = new Vector3(position.x - 4.1f, position.y + 1.1f, position.z + 1.1f); // adjust for offset

            this.transform.position = finalPosition;
            this.transform.rotation = Quaternion.LookRotation(tangent);

            yield return null;
        }
        splinePos = targetSplinePos;
        canCrawlStep = true;
    }

    // this function clamps the camera to the spline tangent direction to limit cam rotation
    private void HandleCameraRotation()
    {
        float pitchInput = lookAmt.y * rotateSpeed * Time.deltaTime;
        float yawInput = lookAmt.x * rotateSpeed * Time.deltaTime;
        cameraPitch -= pitchInput;
        cameraYaw += yawInput;

        if (isCrawling)
        {
            float minPitch = baseCrawlPitch - crawlPitchLimit;
            float maxPitch = baseCrawlPitch + crawlPitchLimit;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

            float minYaw = baseCrawlYaw - crawlYawLimit;
            float maxYaw = baseCrawlYaw + crawlYawLimit;
            cameraYaw = Mathf.Clamp(cameraPitch, minYaw, maxYaw);
            //Vector3 camEuler = cam.transform.localEulerAngles;
            cam.transform.localEulerAngles = new Vector3(cameraPitch, cameraYaw, 0);
        } else
        {
            //Vector3 camEuler = cam.transform.localEulerAngles;
            cam.transform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
        }
    }
}
