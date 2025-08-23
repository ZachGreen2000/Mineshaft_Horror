using UnityEngine;
using System.Collections;
using TMPro;

public class pickUp : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text popUp;
    [Range(0f, 1f)]
    public float textScale;
    [Range(0, 1f)]
    public float textHeight;
    [Range(-1, 1f)]
    public float textWidth;
    [Range(-1, 1f)]
    public float textDepth;

    public int delay;

    [Header("GameObjects")]
    public GameObject candleHoldPos;

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
                if (this.gameObject.tag != "candle")
                {
                    StartCoroutine(itemDelay(delay));
                }else
                {
                    //this.gameObject.SetActive(false);
                    popUp.gameObject.SetActive(false);
                    this.gameObject.transform.SetParent(candleHoldPos.transform);
                    this.gameObject.transform.position = (candleHoldPos.transform.position + new Vector3(0, 0, 0));
                    Quaternion rotationPos = (candleHoldPos.transform.rotation * Quaternion.Euler(50, 0, 0)); // use ueler for adding rotation offset
                    this.gameObject.transform.rotation = rotationPos;
                }
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
            Vector3 desiredPos = new Vector3(currentPos.x + textWidth, currentPos.y + textHeight, currentPos.z + textDepth); // applying positional offset for prompt
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

    // for the delay of object set innactive to allow for animation
    IEnumerator itemDelay(int del)
    {
        yield return new WaitForSeconds(del);
        this.gameObject.SetActive(false);
        popUp.gameObject.SetActive(false);
    }
}
