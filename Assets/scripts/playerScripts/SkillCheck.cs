using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform SC_Bar;
    public RectTransform SC_Zone;
    public RectTransform SC_Input;
    public GameObject SC_Bar_Outline;

    [Header("Settings")]
    public float SC_Speed = 500f;
    private bool isMoving = true;
    private float elapsedTime = 0f;

    [Header("Activations")]
    public bool activateSC = false;
    private bool skillCheckStarted = false;

    [Header("Zone Settings")]
    public float minZoneWidth = 25f;
    public float maxZoneWidth = 60f;

    void Start()
    {
    SC_Bar_Outline.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activateSC = true;
        }
        if (activateSC && !skillCheckStarted)
        {
            StartSkillCheck();
        }
        if (!skillCheckStarted)
            return;

        if (isMoving)
        {
            // ping pongs the input based on distance and time elapsed
            elapsedTime += Time.deltaTime;

            float range = SC_Bar.rect.width - SC_Input.rect.width;
            float x = Mathf.PingPong(elapsedTime * SC_Speed, range);

            Vector2 newPos = new Vector2(-SC_Bar.rect.width / 2f + SC_Input.rect.width / 2f + x, SC_Input.localPosition.y);
            SC_Input.localPosition = newPos;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isMoving = false;
            CheckWinCondition();
        }

    }

    void StartSkillCheck()
    {
        SC_Bar_Outline.SetActive(true);
        skillCheckStarted = true;
        isMoving = true;
        elapsedTime = 0f;
        SetZone();

        Debug.Log("SC Started");
    }
    void CheckWinCondition()
    {
        // checks if the input bar is within the bounds of the zone
        float inputLeft = SC_Input.localPosition.x;
        float inputRight = inputLeft + SC_Input.rect.width;

        float zoneLeft = SC_Zone.localPosition.x;
        float zoneRight = zoneLeft + SC_Zone.rect.width;

        if (inputLeft >= zoneLeft && inputRight <= zoneRight)
        {
            Debug.Log("Yippie!");
            EndSkillCheck();
        }
        else
        {
            Debug.Log("Sad Clown");
            EndSkillCheck();
        }
    }
    void EndSkillCheck()
    {
        isMoving = false;
        activateSC = false;
        skillCheckStarted = false;
        SC_Bar_Outline.SetActive(false);
    }
    void SetZone()
    {
        // sets the size of the zone
        float zoneWidth = Random.Range(minZoneWidth, maxZoneWidth);
        Vector2 size = SC_Zone.sizeDelta;
        size.x = zoneWidth;
        SC_Zone.sizeDelta = size;

        // sets the location of the zone
        float maxOffset = (SC_Bar.rect.width - zoneWidth) / 2f;
        float randomX = Random.Range(-maxOffset, maxOffset);

        Vector2 pos = SC_Zone.localPosition;
        pos.x = randomX;
        SC_Zone.localPosition = pos;
    }
}
