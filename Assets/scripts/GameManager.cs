using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private List<string> pickedItems;

    void Awake()
    {
        Instance = this;
        pickedItems = new List<string>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // this function taked the tag of the item interacted with and uses it to display UI and store items
    public void itemPickUp(string tag)
    {
        pickedItems.Add(tag);
        // UI needs to be made
    }
}
