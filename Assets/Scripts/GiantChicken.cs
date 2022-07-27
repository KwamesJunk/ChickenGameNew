using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantChicken : MonoBehaviour
{
    [SerializeField] GameObject[] eye;
    [SerializeField] GameObject[] laser;
    [SerializeField] GameObject[] leg;
    [SerializeField] GameObject wholeBody;
    GameObject player;
    const float adjustment = 1.0f;
    [SerializeField] GameObject laserImpactEffect;
    [SerializeField] ParticleSystem smokePrefab;
    [SerializeField] GameObject smokePuff;

    ParticleSystem[] smoke;
    bool isLaserActive = false;
    
    const float PI = Mathf.PI;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        //wholeBody = transform.Find("Whole Body").gameObject;
        smoke = new ParticleSystem[2];

        for (int i = 0; i < 2; i++) {
            smoke[i] = Instantiate(smokePrefab, new Vector3(0.0f, -30.0f, 0.0f), Quaternion.Euler(-90.0f, 0.0f, 0.0f));
            smoke[i].Stop();
        }

        //StartCoroutine(InitializeBobVariables());
    }

    float laserCircleCounter = 0.0f;
    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(new Vector3(0.0f, 360.0f, 0.0f) * Time.deltaTime); // It works!!!

        TurnEyes();

        // make smoke come from laser impacts with ground
        LaserSmoke();
        Walk();

        laserCircleCounter += 3.14159f * Time.deltaTime;
    }

    bool ClampYRotation(GameObject obj, float min, float max) 
    {
        Vector3 eulerAngles = obj.transform.localEulerAngles;
        float y = eulerAngles.y;
        bool isClamped = false;

        // to make angle comparisons easier
        if (y > 180.0) {
            y -= 360.0f;
        }

        if (y > max) {
            y = max;
            isClamped = true;
        }
        if (y < min) {
            y = min;
            isClamped = true;
        }

        eulerAngles.y = y;
        obj.transform.localEulerAngles = eulerAngles;

        return isClamped;
    }

    IEnumerator PrintEyeAngles()
    {
        while (true) {
            //print("Left: " + eye[0].transform.localEulerAngles.y);
            //print("Right: " + eye[1].transform.localEulerAngles.y);
            print(" Distance: " + DistanceToPlayer());//(transform.position - player.transform.position).magnitude);
            yield return new WaitForSeconds(1.0f);
        }
    }

    void SetLasersActive(bool active)
    {
        //eye[0].transform.Find("Laser").gameObject.SetActive(active);
        //eye[1].transform.Find("Laser").gameObject.SetActive(active);
        laser[0].gameObject.SetActive(active);
        laser[1].gameObject.SetActive(active);

        if (active) {
            StartLaserSmoke();
        }
        else {
            StopLaserSmoke();
        }

        isLaserActive = active;
    }

    float DistanceToPlayer()
    {
        Vector3 playerPos = player.transform.position + new Vector3(0.0f, adjustment, 0.0f);
        return (transform.position - playerPos).magnitude;
    }

    //IEnumerator LaserImpact() // adjust this to take eye
    //{
    //    GameObject leftLaser = eye[0].transform.Find("Laser").gameObject;
    //    GameObject rightLaser = eye[1].transform.Find("Laser").gameObject;

    //    while (true) {
    //        if (leftLaser.activeSelf) {
    //            Vector3 playerPos = player.transform.position + new Vector3(0.0f, adjustment, 0.0f);
    //            RaycastHit[] hitinfoList = Physics.RaycastAll(eye[0].transform.position, playerPos - eye[0].transform.position);

    //            foreach (RaycastHit hitinfo in hitinfoList) {
    //                if (hitinfo.collider.tag == "Ground") {
    //                    //Instantiate(laserImpactEffect, hitinfo.point, Quaternion.Euler(90,0,0));
    //                    smoke.transform.position = hitinfo.point;
    //                    print("Laser Impact");
    //                    break;
    //                }
    //            }
    //        }

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    void LaserSmoke()
    {
        if (!isLaserActive) return;

        for (int i = 0; i < 2; i++) {
            RaycastHit[] hitinfoList = Physics.RaycastAll(eye[i].transform.position, eye[i].transform.TransformVector(Vector3.back)); // use eye direction to cast ray

            foreach (RaycastHit hitinfo in hitinfoList) {
                if (hitinfo.collider.tag == "Ground") {
                    //smoke[i].transform.position = hitinfo.point;
                    StartSmokePuff(hitinfo.point);
                    //print("Laser Impact");
                    break;
                }
            }
        }
    }

    void StartLaserSmoke()
    {
        for (int i = 0; i < 2; i++) 
            if (!smoke[i].isEmitting) 
                smoke[i].Play();
    }

    void StopLaserSmoke()
    {
        for (int i = 0; i < 2; i++)
            if (smoke[i].isEmitting) 
                smoke[i].Stop();
        
    }

    float lastSmokeTimestamp = 0.0f;
    void StartSmokePuff(Vector3 pos)
    {
        //if (Time.time - lastSmokeTimestamp > 0.1f) {
            Destroy(Instantiate(smokePuff, pos, Quaternion.identity), 2.1f);
            lastSmokeTimestamp = Time.time;
        //}
    }

    // Offset eye direction to make lasers move in circles
    Vector3 LaserCircleOffset(float phase)
    {
        return new Vector3(Mathf.Sin(laserCircleCounter + phase), 0.0f, 2.0f * Mathf.Cos(laserCircleCounter + phase));
    }

    void TurnEyes()
    {
        Vector3 playerPos = player.transform.position + new Vector3(0.0f, adjustment, 0.0f);

        // look at a point just above the player's transform position (by adjustment), then clamp
        Vector3 lookDir = (eye[0].transform.position - playerPos) + eye[0].transform.position + LaserCircleOffset(0.0f);
        eye[0].transform.LookAt(lookDir);
        bool clampedLeft = ClampYRotation(eye[0], -27.0f, 39.0f);

        lookDir = (eye[1].transform.position - playerPos) + eye[1].transform.position + LaserCircleOffset(3.14159f);
        eye[1].transform.LookAt(lookDir);
        bool clampedRight = ClampYRotation(eye[1], -38.0f, 30.0f);

        // Only fire if player is close enough.
        if (DistanceToPlayer() < 30.0f) {
            SetLasersActive(!clampedLeft && !clampedRight);
        }
        else {
            SetLasersActive(false);
        }
    }

    float walkCounter = 0.0f;
    void Walk() // TODO: make chicken bob as she walks
    {
        float xAngle = 45 * Mathf.Cos(walkCounter * PI);
        const float amplitude = -0.1f;

        leg[0].transform.localRotation = Quaternion.Euler(new Vector3(xAngle, -90.0f, 0.0f));
        leg[1].transform.localRotation = Quaternion.Euler(new Vector3(-xAngle, -90.0f, 0.0f));

        //Bob
        float plusMinus = xAngle / Mathf.Abs(xAngle);
        wholeBody.transform.localPosition = new Vector3(0.0f, amplitude*Mathf.Cos((xAngle * PI / 180f + PI/2)*2), 0.0f); // somehow this works

        walkCounter += Time.deltaTime;
    }

    //IEnumerator InitializeBobVariables()
    //{
    //    yield return new WaitForSeconds(2.0f);

    //    baseY = GetComponent<Rigidbody>().position.y;
    //}
}
