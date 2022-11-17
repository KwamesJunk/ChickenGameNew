using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComeApart : MonoBehaviour
{
    [SerializeField]GameObject[] breakParts;
   
    public void Execute()
    {
        //if (broken) return;
        //broken = true;

        //print("comeapart");
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        foreach (GameObject bodyPart in breakParts) {
            if (!bodyPart) continue; // in case bodyPart has been destroyed after fast fade

            Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            bodyPart.transform.parent = null;
            rb.constraints = RigidbodyConstraints.None;

            // get collider, whatever it may be, and enable it
            Collider collider = bodyPart.GetComponent<Collider>();
            collider.enabled = true;

            // randomize explosion a bit
            float mag = Random.Range(3.0f, 5.0f);
            bodyPart.GetComponent<Rigidbody>().AddForce(mag * randomUpVector(), ForceMode.Impulse);
            Destroy(bodyPart, 5.0f);
        }

        //Destroy(gameObject, 5.0f);
        
    }

    
    public void ExecuteNoDestroy(float minForce, float maxForce, float lifeTime)
    {
        //if (broken) return;
        //broken = true;

        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        foreach (GameObject bodyPart in breakParts) {
            Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            bodyPart.transform.parent = null;

            // get collider, whatever it may be, and enable it
            Collider collider = bodyPart.GetComponent<Collider>();
            collider.enabled = true;

            // randomize explosion a bit
            float mag = Random.Range(minForce, maxForce); // 3.0, 5.0
            bodyPart.GetComponent<Rigidbody>().AddForce(mag * randomUpVector(), ForceMode.Impulse);
            Destroy(bodyPart, lifeTime);
        }
    }

    Vector3 randomUpVector()
    {
        return new Vector3(Random.Range(-0.2f, 0.2f), 1.0f, Random.Range(-0.2f, 0.2f));
    }
}
