using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenLaser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == Global.PLAYER_TAG) {
            //print("Giant Chicken Laser hit Player!");
            other.GetComponent<HitPoints>().Decrement();
        }
    }
}
