using UnityEngine;

public class cutsceneTrigger : MonoBehaviour
{
    public SplineMover mover;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mover.StartMoving();
        }
    }
}
