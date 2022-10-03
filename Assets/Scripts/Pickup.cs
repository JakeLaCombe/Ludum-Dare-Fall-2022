using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Start is called before the first frame update
    public PickupTypes pickupType;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
         if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.AddPickup(pickupType);
            Destroy(this.gameObject);
        }
    }
}


public enum PickupTypes
{
    INITIAL,
    FORCE_FIELD,
}