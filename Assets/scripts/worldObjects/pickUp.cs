using UnityEngine;
using TMPro;

public class pickUp : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text popUp;
    [Range(0f, 1f)]
    public float textScale;
    [Range(0, 1f)]
    public float textHeight;

    private GameObject player;

    void Start()
    {
        popUp.gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (popUp.gameObject.activeSelf)
        {
            // rotating popup to match players looking position
            Vector3 direction = this.gameObject.transform.position - player.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            popUp.transform.rotation = Quaternion.Slerp(popUp.transform.rotation, targetRotation, 2 * Time.deltaTime);

            // send item reference based on input and item tag for item collection
            if (PlayerMovement.Instance.interactAction.WasPressedThisFrame())
            {
                // send item reference to game manager and set object innactive
                string itemTag = this.gameObject.tag;
                GameManager.Instance.itemPickUp(itemTag);
                // ----- Add courotine to delay this section below ------ //
                this.gameObject.SetActive(false);
                popUp.gameObject.SetActive(false);
            }
        }
    }
    
    // trigger zone for displaying text prompt and handling item pickup
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            popUp.gameObject.SetActive(true);
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 desiredPos = new Vector3(currentPos.x, currentPos.y + textHeight, currentPos.z); // applying positional offset for prompt
            popUp.transform.position = desiredPos;
            // edit scale of prompt
            popUp.transform.localScale = new Vector3(textScale, textScale, textScale);

        }
    }
    // on exit of trigger zone make popUp invisible
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            popUp.gameObject.SetActive(false);
        }
    }
}
