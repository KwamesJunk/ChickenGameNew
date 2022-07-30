using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigChickenAttack : MonoBehaviour
{
    BigChicken chicken;
    //bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        chicken = transform.parent.GetComponent<BigChicken>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Transform parentTransform = other.transform.root;

        if (parentTransform.gameObject == transform.root.gameObject) {
            //print("Hit Self");
            return;
        }
        if (other.tag == "Chicken") {
            if (other.gameObject != gameObject) {
                if (parentTransform.tag == "Chicken") {
                    //print("Hit a chicken.");
                    ChickenBase chickenController = parentTransform.GetComponent<ChickenBase>();
                    chickenController.TakeDamage(gameObject);
                }
            }
        }
    }

}
