using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenOfDeath : ChickenBase
{
    //[SerializeField] GameObject[] breakParts;
    [SerializeField] public GameObject torso;
    float idleRadius = 5.0f;
    [SerializeField] float attackChargeTime = 3.0f;
    [SerializeField] Material attackMaterial;
    [SerializeField] Material regularMaterial;
    HitPoints hp;
    bool killedByPlayer = false;

    enum State
    {
        IDLE, CHASING, DEAD, PRE_ATTACK, ATTACKING, SPAWNING
    }

    float timeKeeper = 0.0f;
    float hopDelay = 2.0f;
    State state = State.SPAWNING;
    Vector3 facing;
    Color baseColour;
    Vector3 attackVector;
    const float INVINCIBILITY_TIME = 0.5f;
    float lastTimeHit = 0.0f;

    //private void Awake()
    //{
    //    player = GameObject.Find("Player");

    //    if (!player) print("Player not found");

    //    //life = 1;
    //}

    // Start is called before the first frame update
    private void Start()
    {
        // Make this static
        baseColour = torso.GetComponent<MeshRenderer>().material.color;

        hp = GetComponent<HitPoints>();
        //hp.onZeroOrLess += Die;
        hp.Set(1);
    }

    //IEnumerator TimedDeath(float deathTime) {
    //    yield return new WaitForSeconds(deathTime);

    //    ComeApart();
    //}

    // Update is called once per frame
    public void Update()
    {
        if (transform.position.y < -15.0f) Destroy(gameObject);

        if (hp.Get() <= 0) {
            Die();
            return;
        }

        switch (state) {
            case State.IDLE:
                if (player == null) print("didn't work");
                if (Vector3.Distance(transform.position, player.transform.position) >= idleRadius) {
                    ChangeState(State.CHASING);
                    break;
                }

                // just hop at first
                if (timeKeeper == 0.0f) {
                    Vector3 direction = new Vector3(0.0f, 1.0f, 1.0f);
                    transform.Rotate(new Vector3(0.0f, Random.Range(-90.0f, 90.0f), 0.0f));
                    
                    Hop(transform.TransformVector(direction), Random.Range(0.5f, 3.0f));
                    hopDelay = Random.Range(1.0f, 3.0f);
                }

                RotateTowardTargetSmoothly(player.transform.position, 15.0f);

                timeKeeper += Time.deltaTime;

                if (timeKeeper > hopDelay) {
                    timeKeeper = 0.0f;
                    ChangeState(State.PRE_ATTACK);
                }
                break;
            case State.CHASING:

                if (Vector3.Distance(transform.position, player.transform.position) < idleRadius) {
                    ChangeState(State.IDLE);
                    break;
                }

                if (timeKeeper == 0.0f) {
                    Vector3 direction = player.transform.position - transform.position;

                    //transform.LookAt(target.transform.position + new Vector3(0.0f, 2.0f, 0.0f));
                    Hop(direction, 5.0f);
                    hopDelay = Random.Range(1.0f, 3.0f);
                }

                RotateTowardTargetSmoothly(player.transform.position, 15.0f);

                timeKeeper += Time.deltaTime;

                if (timeKeeper > hopDelay) {
                    timeKeeper = 0.0f;
                }

                break;
            case State.PRE_ATTACK:
                if (timeKeeper == 0.0f) {  
                    attackVector = player.transform.position - transform.position;
                    if (attackVector.sqrMagnitude < 0.1f) {
                        attackVector = facing;
                    }
                }

                SetBodyColour(InterpolateColour(timeKeeper / attackChargeTime));

                timeKeeper += Time.deltaTime;

                if (timeKeeper >= attackChargeTime) {
                    ChangeState(State.ATTACKING);
                }
                break;
            case State.ATTACKING:
                if (timeKeeper == 0.0f) {
                    hopDelay = 2.0f;
                    Hop(attackVector, 7.5f);
                }

                timeKeeper += Time.deltaTime;

                if (timeKeeper > 1.0f) {
                    ChangeState(State.IDLE);
                    SetBodyColour(baseColour);
                }
                break;
            case State.SPAWNING:
                if (timeKeeper >= 2.0f) {// time to fall to the ground

                    ChangeState(State.IDLE);
                }

                timeKeeper += Time.deltaTime;
                break;
        }

        

        facing = transform.TransformVector(Vector3.forward);
        facing.x = facing.z = 0;
    }

    //void ComeApart()
    //{
    //    //print("comeapart");
    //    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

    //    foreach (GameObject bodyPart in breakParts) {
    //        Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
    //        rb.isKinematic = false;
    //        bodyPart.transform.parent = null;

    //        // get collider, whatever it may be, and enable it
    //        Collider collider = bodyPart.GetComponent<Collider>();
    //        collider.enabled = true;

    //        // randomize explosion a bit
    //        float mag = Random.Range(3.0f, 5.0f);
    //        bodyPart.GetComponent<Rigidbody>().AddForce(mag * randomUpVector(), ForceMode.Impulse);
    //        Destroy(bodyPart, mag);
    //    }

    //    Destroy(gameObject);
    //}

    Vector3 randomUpVector()
    {
        return new Vector3(Random.Range(-0.2f, 0.2f), 1.0f, Random.Range(-0.2f, 0.2f));
    }

    //const float INVINCIBILITY_TIME = 0.5f; // already in chickencontroller
    //float lastTimeHit = 0.0f;
    public override void TakeDamage(GameObject killer)
    {
        if (player.GetComponent<PlayerController2>().IsGameOver()) return;

        if (state != State.DEAD && Time.time - lastTimeHit > INVINCIBILITY_TIME) {
            if (hp.Get() <= 0) return;

            lastTimeHit = Time.time;
            hp.Decrement();

            if (killer.tag == "Player") {
                killedByPlayer = true;
                print(name + " was killed by Player");
            }

            print("TakeDamage: " + name);

            if (hp.Get() <= 0) {
                //Die();
                return;
            }

            // From here on, the chicken is not dead.

            print("Tag: "+killer.tag);
            // Let the Chicken Spawner know if the player is the one who is the killer
            if (killer.tag == "Player") {
                //print("Player hit " + name);
                

                Vector3 hitForce = (transform.position - killer.transform.position);
                hitForce.Normalize();
                hitForce.y = 1.0f;
                hitForce *= 500;
                GetComponent<Rigidbody>().AddForce(hitForce, ForceMode.Impulse);
                print(hitForce);

                //temporary kludge
                BigChicken bgc = GetComponent<BigChicken>();
                if (bgc) {
                    bgc.SetToIdle();
                }
            }
        }
    }

    void Die()
    {
        if (state == State.DEAD) return;

        print("Die ("+name+")");
        state = State.DEAD;
        GetComponent<ComeApart>().Execute();
        GetComponent<FadeParts>().StartFading();
        Destroy(gameObject, 5.1f);


        ChickenSpawner spawner = GetComponent<ChickenBase>().GetChickenSpawner();

        if (spawner != null) {
            if (killedByPlayer) {
                spawner.IncrementKillCount();
            }
        }
        else {
            print("ChickenSpawner unassigned!");
        }

        if (Random.Range(0, 10) == 0) {
            SpawnLifeChicken();
        }
    }

    void Hop(Vector3 targetDirection, float jumpPower)
    {
        Vector3 direction = Vector3.Normalize(targetDirection) + Vector3.up;

        GetComponent<Rigidbody>().AddForce(jumpPower * direction, ForceMode.Impulse);
    }

    void RotateTowardTargetSmoothly(Vector3 targetPosition, float maxDegPerSec)
    {
        //print("Rotating Smoothly");
        Vector3 targetPositionn = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPositionn);
    }

    void SetBodyColour(Color colour)
    {
        torso.GetComponent<MeshRenderer>().material.color = colour;
    }

    void ChangeState(State newState)
    {
        state = newState;
        timeKeeper = 0.0f;
    }

    //public Material GetMaterial() // in superclass
    //{
    //    return torso.GetComponent<MeshRenderer>().material;
    //}

    Color InterpolateColour(float ratio)
    {
        float r = baseColour.r + (0.99f-baseColour.r) * ratio;
        float g = baseColour.g - (baseColour.g-0.01f) * ratio;
        float b = baseColour.b - (baseColour.b-0.01f) * ratio;

        return new Color(r, g, b);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && state == State.ATTACKING) {
            if (collider.GetComponent<HitPoints>()) {
                //print("Hp is " + collider.GetComponent<HitPoints>().Get());
                collider.GetComponent<HitPoints>().Decrement(0);
                collider.GetComponent<PlayerController2>().OnDamaged();
            }
            else {
                print("No hitpoint component");
            }
             // 
        }
    }

    void SpawnLifeChicken()
    {
        Instantiate(lifeChickenPrefab, transform.position, transform.rotation);
    }
}
