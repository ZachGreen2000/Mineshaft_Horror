using UnityEngine;

public class crawlTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider obj)
    {
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
