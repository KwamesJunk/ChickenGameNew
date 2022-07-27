using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChickenBase : MonoBehaviour
{ 
    protected GameObject player;
    
    protected ChickenSpawner chickenSpawner;

    public abstract void Update();
    public abstract void TakeDamage(GameObject killer);


    private void Awake()
    {
        player = GameObject.Find("Player");

        if (!player) print("Player not found");
    }

    public void SetChickenSpawner(ChickenSpawner spawner)
    {
        chickenSpawner = spawner;
    }

    public ChickenSpawner GetChickenSpawner() // temporary kludge
    {
        return chickenSpawner;
    }

    public void SetLife(int newLife)
    {
        //life = newLife;
        GetComponent<HitPoints>().Set(newLife);
    }

    //enum ChickenState
    //{
    //    IDLE, CHASING, DEAD, PRE_ATTACK, ATTACKING, SPAWNING
    //}

    //protected float timeKeeper = 0.0f;
    //ChickenState state = ChickenState.SPAWNING;
    //Vector3 facing;
    //Color baseColour;
    //Vector3 attackVector;

    //protected const float INVINCIBILITY_TIME = 0.5f;
    //protected float lastTimeHit = 0.0f;

    

    // Start is called before the first frame update
    //private void Start()
    //{
    //    // Make this static
    //    //torso = breakParts[0];
    //    //baseColour = torso.GetComponent<MeshRenderer>().material.color;

    //    //if (transform.localScale.x == 1.0f) StartCoroutine(TimedDeath(10.0f)); // kill it after ten seconds (remove this later)
    //}

    //IEnumerator TimedDeath(float deathTime) {
    //    yield return new WaitForSeconds(deathTime);

    //    ComeApart();
    //}

    // Update is called once per frame
    //public void Update() { 
    //{
    //    if (transform.position.y < -15.0f) Destroy(gameObject);

    //    switch (state) {
    //        case ChickenState.IDLE:
    //            if (player == null) print("didn't work");
    //            if (Vector3.Distance(transform.position, player.transform.position) >= idleRadius) {
    //                ChangeState(ChickenState.CHASING);
    //                break;
    //            }

    //            // just hop at first
    //            if (timeKeeper == 0.0f) {
    //                Vector3 direction = new Vector3(0.0f, 1.0f, 1.0f);
    //                transform.Rotate(new Vector3(0.0f, Random.Range(-90.0f, 90.0f), 0.0f));

    //                Hop(transform.TransformVector(direction), Random.Range(0.5f, 3.0f));
    //                hopDelay = Random.Range(1.0f, 3.0f);
    //            }

    //            RotateTowardTargetSmoothly(player.transform.position, 15.0f);

    //            timeKeeper += Time.deltaTime;

    //            if (timeKeeper > hopDelay) {
    //                timeKeeper = 0.0f;
    //                ChangeState(ChickenState.PRE_ATTACK);
    //            }
    //            break;
    //        case ChickenState.CHASING:

    //            if (Vector3.Distance(transform.position, player.transform.position) < idleRadius) {
    //                ChangeState(ChickenState.IDLE);
    //                break;
    //            }

    //            if (timeKeeper == 0.0f) {
    //                Vector3 direction = player.transform.position - transform.position;

    //                //transform.LookAt(target.transform.position + new Vector3(0.0f, 2.0f, 0.0f));
    //                Hop(direction, 5.0f);
    //                hopDelay = Random.Range(1.0f, 3.0f);
    //            }

    //            RotateTowardTargetSmoothly(player.transform.position, 15.0f);

    //            timeKeeper += Time.deltaTime;

    //            if (timeKeeper > hopDelay) {
    //                timeKeeper = 0.0f;
    //            }

    //            break;
    //        case ChickenState.PRE_ATTACK:
    //            if (timeKeeper == 0.0f) {  
    //                attackVector = player.transform.position - transform.position;
    //                if (attackVector.sqrMagnitude < 0.1f) {
    //                    attackVector = facing;
    //                }
    //            }

    //            SetBodyColour(InterpolateColour(timeKeeper / attackChargeTime));

    //            timeKeeper += Time.deltaTime;

    //            if (timeKeeper >= attackChargeTime) {
    //                ChangeState(ChickenState.ATTACKING);
    //            }
    //            break;
    //        case ChickenState.ATTACKING:
    //            if (timeKeeper == 0.0f) {
    //                hopDelay = 2.0f;
    //                Hop(attackVector, 7.5f);
    //            }

    //            timeKeeper += Time.deltaTime;

    //            if (timeKeeper > 1.0f) {
    //                ChangeState(ChickenState.IDLE);
    //                SetBodyColour(baseColour);
    //            }
    //            break;
    //        case ChickenState.SPAWNING:
    //            if (timeKeeper >= 2.0f) {// time to fall to the ground

    //                ChangeState(ChickenState.IDLE);
    //            }

    //            timeKeeper += Time.deltaTime;
    //            break;
    //    }



    //    facing = transform.TransformVector(Vector3.forward);
    //    facing.x = facing.z = 0;
    //}

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

    //Vector3 randomUpVector()
    //{
    //    return new Vector3(Random.Range(-0.2f, 0.2f), 1.0f, Random.Range(-0.2f, 0.2f));
    //}


   
    //public virtual void TakeDamage(GameObject killer) // pass Vector3 force as a parameter
    //{
    //    //print("takedamage");
    //    if (state != ChickenState.DEAD && Time.time - lastTimeHit > INVINCIBILITY_TIME) {
    //        lastTimeHit = Time.time;
    //        //--life;

    //        HitPoints hp = GetComponent<HitPoints>(); // a kludge until I think of something better
    //        hp.Decrement();

    //        //if (life <= 0) {
    //        if (hp.Get() <= 0) {
    //            state = ChickenState.DEAD;
    //            GetComponent<ComeApart>().Execute();

    //            //if (chickenSpawner) {             // This avoids double-increments. ChickenOfDeath class also increments kill count.
    //            //    chickenSpawner.IncrementKillCount();
    //            //}
    //            //else {
    //            //    print("ChickenSpawner unassigned!");
    //            //}

    //            return;
    //        }

    //        // From here on, the chicken is not dead.

    //        //print("Tag: "+killer.tag);
    //        // Let the Chicken Spawner know if the player is the one who is the killer
    //        if (killer.tag == "Player") {
    //            //print("Player hit " + name);


    //            Vector3 hitForce = (transform.position - killer.transform.position);
    //            hitForce.Normalize();
    //            hitForce.y = 1.0f;
    //            hitForce *= 500;
    //            GetComponent<Rigidbody>().AddForce(hitForce, ForceMode.Impulse);
    //            print(hitForce);

    //            //temporary kludge
    //            BigChickenController bgc = GetComponent<BigChickenController>();
    //            if (bgc) {
    //                bgc.SetToIdle();
    //            }
    //        }
    //    }
    //}

    //void Hop(Vector3 targetDirection, float jumpPower)
    //{
    //    Vector3 direction = Vector3.Normalize(targetDirection) + Vector3.up;

    //    GetComponent<Rigidbody>().AddForce(jumpPower * direction, ForceMode.Impulse);
    //}

    //void RotateTowardTargetSmoothly(Vector3 targetPosition, float maxDegPerSec)
    //{
    //    //print("Rotating Smoothly");
    //    Vector3 targetPositionn = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    //    transform.LookAt(targetPositionn);
    //}

    //void SetBodyColour(Color colour)
    //{
    //    torso.GetComponent<MeshRenderer>().material.color = colour;
    //}

    //void ChangeState(ChickenState newState)
    //{
    //    state = newState;
    //    timeKeeper = 0.0f;
    //}

    //public Material GetMaterial()
    //{
    //    return torso.GetComponent<MeshRenderer>().material;
    //}

    //Color InterpolateColour(float ratio)
    //{
    //    float r = baseColour.r + (0.99f-baseColour.r) * ratio;
    //    float g = baseColour.g - (baseColour.g-0.01f) * ratio;
    //    float b = baseColour.b - (baseColour.b-0.01f) * ratio;

    //    return new Color(r, g, b);
    //}

    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.tag == "Player" && state == ChickenState.ATTACKING) {
    //        //print("Hit the player!");
    //    }
    //}

    

    //public void ComeApartNoDestroy(float minForce, float maxForce, float lifeTime)
    //{
    //    //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

    //    foreach (GameObject bodyPart in breakParts) {
    //        Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
    //        rb.isKinematic = false;
    //        bodyPart.transform.parent = null;

    //        // get collider, whatever it may be, and enable it
    //        Collider collider = bodyPart.GetComponent<Collider>();
    //        collider.enabled = true;

    //        // randomize explosion a bit
    //        float mag = Random.Range(minForce, maxForce); // 3.0, 5.0
    //        bodyPart.GetComponent<Rigidbody>().AddForce(mag * randomUpVector(), ForceMode.Impulse);
    //        Destroy(bodyPart, lifeTime);
    //    }
    //}
}
