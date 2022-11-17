using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeChicken : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = transform.position + (Vector3.down * 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 1f) {
            transform.position = transform.position + (Vector3.up * Time.deltaTime);
        }
        transform.GetChild(0).localPosition = new Vector3(0.0f, Mathf.Cos(Mathf.PI * Time.time) * 0.2f, 0.0f);
        transform.GetChild(0).Rotate(new Vector3(0.0f, 90 * Time.deltaTime, 0.0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Global.PLAYER_TAG) {
            other.GetComponent<HitPoints>().SetToMax();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject, 3.0f);
        }

        
    }
}
