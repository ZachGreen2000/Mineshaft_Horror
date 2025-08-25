using UnityEngine;

public class MineFall : MonoBehaviour
{
    public GameObject floor;
    public void OnTriggerEnter(Collider col)
    {
        Destroy(floor);
    }
}
