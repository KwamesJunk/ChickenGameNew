using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorChicken : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject targetEffect;
    [SerializeField] int damage = 0;
    Vector3 rayDirection;
    Vector3 impactPoint;

    // Start is called before the first frame update
    void Start()
    {
        rayDirection = transform.TransformVector(Vector3.up);

        RaycastHit[] hitInfoList = Physics.RaycastAll(transform.position, rayDirection);

        //print("Before: "+transform.position);

        foreach (RaycastHit hitInfo in hitInfoList) {
            if (hitInfo.collider.tag == "Ground") {

                //playerpos-hitinfo.point -> add this to meteor chiken's initital position (except y component)
                Vector3 playerPos = GameObject.Find("Player").transform.position;
                Vector3 offset = playerPos - hitInfo.point;
                offset.y = 0.0f;
                transform.position = transform.position + offset;
                impactPoint = hitInfo.point + offset;

                GameObject targetSFX = Instantiate(targetEffect, impactPoint, Quaternion.identity);
                Destroy(targetSFX, 3.0f);
            }
        }
    }

    bool dead = false;
    float deadTime = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (dead) return;

        transform.GetChild(0).Rotate(720 * Time.deltaTime * Vector3.up);

        transform.position += (rayDirection * 12f * Time.deltaTime);

        if (transform.position.y <= 1.0f) {
            //transform.GetChild(0).gameObject.SetActive(false);
            Destroy(Instantiate(explosion, impactPoint, Quaternion.identity), 2.1f);
            Destroy(Instantiate(explosion, impactPoint + (Vector3.forward*0.2f), Quaternion.identity), 1.5f);
            Destroy(Instantiate(explosion, impactPoint + (Vector3.left*0.2f), Quaternion.identity), 1.75f);
            GetComponent<AudioSource>().Play();
            Destroy(gameObject, 2.2f);
            dead = true;
            deadTime = Time.time;
            //GetComponent<ChickenController2>().ComeApartNoDestroy(6.0f, 12.0f, 2.0f);
            GetComponent<ComeApart>().ExecuteNoDestroy(6.0f, 12.0f, 2.0f);
            GetComponent<FadeParts>().StartFading();

            GetComponent<SphereCollider>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - deadTime < 0.5f) { // only do damage for 0.5s
            HitPoints hp = other.GetComponent<HitPoints>();
            
            if (hp) {
                hp.Decrement(damage);

                if (other.tag == Global.PLAYER_TAG) {
                    other.GetComponent<PlayerController2>().OnKnockedDown();
                }
            }
            
        }
    }

    private void OnDestroy()
    {
        
    }
}
