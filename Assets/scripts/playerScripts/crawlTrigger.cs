using UnityEngine;

public class crawlTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("crawlZone"))
        {
            Debug.Log("Collided");
        }
    }
}
