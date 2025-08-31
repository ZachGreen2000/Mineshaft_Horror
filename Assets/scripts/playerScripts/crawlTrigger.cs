using UnityEngine;

public class crawlTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("entranceZone") && GameManager.Instance.canCave == true) { PlayerMovement.Instance.moveToRope(); return; } // exit function if met
        if (obj.CompareTag("crawlZone"))
        {
            if (PlayerMovement.Instance.isCrawling != true)
            {
                PlayerMovement.Instance.Crawl();
            }
            Debug.Log("Collided");
        }
    }
}
