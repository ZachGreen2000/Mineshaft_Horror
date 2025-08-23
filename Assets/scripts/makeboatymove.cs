using UnityEngine;

public class makeboatymove : MonoBehaviour
{
    public bool inBoat = false;
    public bool isRowing = false;
    public float thrustForce = 100f;
    Rigidbody boatRB;

    void OnTriggerEnter(Collider collider)
    {
        //this dont need a comment 
        if (collider.gameObject.CompareTag("Player"))
        {
            inBoat = true;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gets rigidbody 
        boatRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if player in boat and key press, add force forward
        if (inBoat)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                //isRowing = true;  CHANGE THIS TO A COROUTINE TO USE A TIMER!!!
                boatRB.AddForce(transform.forward * thrustForce);
            }
        }
        
    }
}
