using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigChicken : ChickenBase
{
    [SerializeField] int life;
    float peckAngle;
    Transform baseAnimal;
    State state;
    float timeKeeper = 0.0f;
    Rigidbody rigidBody;
    [SerializeField] GameObject impact;
    [SerializeField] GameObject beak;
    //float originalScale;
    [SerializeField] GameObject flapWingLeft;
    [SerializeField] GameObject flapWingRight;
    HitPoints hp;
    const float INVINCIBILITY_TIME = 0.5f;
    float lastTimeHit = 0.0f;

    enum State
    {
        IDLE, CHASING, DEAD, PRE_ATTACK, ATTACKING, SPAWNING, PECK, PECK_REVERSE, ERROR
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
        rigidBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        baseAnimal = transform.Find("BaseAnimal");
        state = State.SPAWNING;
        peckAngle = 0.0f;
        //originalScale = transform.localScale.y;
        //SetScale(0.5f);
        //GetComponent<ChickenController2>().SetLife(5);
        hp = GetComponent<HitPoints>();
        hp.Set(5);


        Vector3 eulers = baseAnimal.transform.rotation.eulerAngles;
        //print("Base Animal X angle:" + eulers.x);
    }

    float flapCounter = 0.0f;

    // Update is called once per frame
    public override void Update()
    {
        switch (state) {
            case State.PECK:
                peckAngle -= 360.0f * Time.deltaTime;

                RotateBaseAnimal(-360.0f * Time.deltaTime);

                if (peckAngle < -60.0f) {
                    state = State.PECK_REVERSE;
                    ImpactEffect();
                }
                break;
            case State.PECK_REVERSE:
                peckAngle += 120.0f * Time.deltaTime;

                RotateBaseAnimal(120.0f * Time.deltaTime);

                if (peckAngle > 0.0f) {
                    //state = ChickenState.IDLE;
                    state = State.CHASING;
                    //SetBaseAnimalXRotation(0.0f);

                }
                break;
            case State.IDLE:
                if (timeKeeper > 3.0f) {

                    timeKeeper = 0.0f;
                    state = State.CHASING;

                    // reset peck position
                    SetBaseAnimalXRotation(0.0f); 
                    peckAngle = 0.0f;
                }

                timeKeeper += Time.deltaTime;
                break;
            case State.CHASING:
                Vector3 direction = player.transform.position - transform.position;
                direction.y = 0;

                LookAtPlayer();
                Flap();

                if (direction.magnitude < 5.0f) {
                    state = State.PECK;
                    SetWingAngles(15, -15);
                    //LookAtPlayer();
                }

                direction.Normalize();
                direction *= 5.0f;
                direction.y = rigidBody.velocity.y;

                rigidBody.velocity = direction;

                
                break;
            case State.SPAWNING:
                Flap();

                if (transform.position.y < 1.0f) {
                    state = State.CHASING;
                    //state = ChickenState.ERROR;
                    SetWingAngles(15, -15);
                }
                break;
        }


        //Flap();
        
    }

    public override void TakeDamage(GameObject attacker) // ass Vector3 force as a parameter
    {
        //print("takedamage");
        if (state != State.DEAD && Time.time - lastTimeHit > INVINCIBILITY_TIME) {
            lastTimeHit = Time.time;
            //--life;

            HitPoints hp = GetComponent<HitPoints>(); // a kludge until I think of something better
            hp.Decrement();

            //if (life <= 0) {
            if (hp.Get() <= 0) {
                state = State.DEAD;
                GetComponent<ComeApart>().Execute();
                GetComponent<FadeParts>().StartFading();

                //if (chickenSpawner) {             // This avoids double-increments. ChickenOfDeath class also increments kill count.
                //    chickenSpawner.IncrementKillCount();
                //}
                //else {
                //    print("ChickenSpawner unassigned!");
                //}

                return;
            }

            // From here on, the chicken is not dead.

            //print("Tag: "+killer.tag);
            // Let the Chicken Spawner know if the player is the one who is the killer
            if (attacker.tag == "Player") {
                //print("Player hit " + name);


                Vector3 hitForce = (transform.position - attacker.transform.position);
                hitForce.Normalize();
                hitForce.y = 1.0f;
                hitForce *= 500;
                GetComponent<Rigidbody>().AddForce(hitForce, ForceMode.Impulse);
                //print(hitForce);

                //temporary kludge 
                // 2022/7/13: What the Hell is this? What?
                BigChicken bgc = GetComponent<BigChicken>();
                if (bgc) {
                    bgc.SetToIdle();
                }
            }
        }
    }

    void Flap()
    {
        flapCounter += Mathf.PI * 4 * Time.deltaTime;

        //if (Input.GetKey(KeyCode.C)) {
        //    flapCounter += 2;
        //    print("Angle: " + flapCounter);
        //}
        //if (Input.GetKey(KeyCode.Z)) {
        //    flapCounter -= 2;
        //    print("Angle: " + flapCounter);
        //}

        float zRotation = -85 + 65 * Mathf.Cos(flapCounter);

        SetWingAngles(-zRotation, zRotation);

        //Vector3 localRotR = flapWingRight.transform.localRotation.eulerAngles;
        //Vector3 localRotL = flapWingLeft.transform.localRotation.eulerAngles;
        //print("zR:" + localRotR.z);
        //print("zL:" + localRotL.z);
        //localRotR.z = zRotation;
        //localRotL.z = -zRotation;
        //flapWingRight.transform.localRotation = Quaternion.Euler(localRotR);
        //flapWingLeft.transform.localRotation = Quaternion.Euler(localRotL);

        // Rotate to check
        //Vector3 rot = transform.rotation.eulerAngles;
        //rot.y += 180.0f * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(rot);
    }

    void SetWingAngles(float leftAngle, float rightAngle)
    {
        Vector3 localRotR = flapWingRight.transform.localRotation.eulerAngles;
        Vector3 localRotL = flapWingLeft.transform.localRotation.eulerAngles;
        //print("zR:" + localRotR.z);
        //print("zL:" + localRotL.z);
        localRotR.z = rightAngle;
        localRotL.z = leftAngle;
        flapWingRight.transform.localRotation = Quaternion.Euler(localRotR);
        flapWingLeft.transform.localRotation = Quaternion.Euler(localRotL);
    }

    void RotateBaseAnimal(float degrees)
    {
        baseAnimal.transform.Rotate(new Vector3(degrees, 0.0f, 0.0f));
    }

    void SetBaseAnimalXRotation(float degrees)
    {
        Vector3 eulers = baseAnimal.transform.rotation.eulerAngles;


        baseAnimal.transform.rotation = Quaternion.Euler(degrees, eulers.y, eulers.z);
    }

    void LookAtPlayer()
    {
        Vector3 tartgetPos = player.transform.position;
        tartgetPos.y = transform.position.y;
        transform.LookAt(tartgetPos);
    }

    void ImpactEffect()
    {
        RaycastHit hitInfo;

        Physics.Raycast(beak.transform.position, Vector3.down, out hitInfo);
        GameObject impactEffect = Instantiate(impact, hitInfo.point, Quaternion.identity);
        Destroy(impactEffect, 0.5f);
    }

    void SetScale(float newScale)
    {
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void SetToIdle()
    {
        state = State.IDLE;
    }

    void RotateTowardTargetSmoothly(Vector3 targetPosition, float maxDegPerSec)
    {
        //print("Rotating Smoothly");
        Vector3 targetPositionn = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPositionn);
    }
}