using System.Collections;
using UnityEngine;


public class ChickenController : MonoBehaviour
{
    //[SerializeField] GameObject leftLeg;
    //[SerializeField] GameObject rightLeg;
    //[SerializeField] GameObject torso;
    //[SerializeField] GameObject leftWing;
    //[SerializeField] GameObject rightWing;
    [SerializeField] GameObject[] breakParts;
    [SerializeField] float speed = 0.5f;
    [SerializeField] GameObject target;
    [SerializeField] float distance = 0.0f;
    [SerializeField] Material attackMaterial;
    [SerializeField] Material regularMaterial;
    
    float hopDelay; // how long to wait before hopping next (random)
    Vector3 hopStartPos;
    ChickenState state;
    Vector3 targetDirection;
    
    enum ChickenState
    {
        IDLE, WALKING, RUNNING, HUNTING, DYING, DEAD, TAKE_DAMAGE, PRE_HOP, HOPPING, PRE_ATTACK, ATTACKING
    }

    private void Awake()
    {
        state = ChickenState.PRE_HOP;
        timeKeeper = 0.0f;
        //currentVel = vel = Vector3.forward;
        //hopTimer = 0.0f; 
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(3, 6);
        Physics.IgnoreLayerCollision(3, 3);
        //StartWalking();
        //StartHunting();
        //TakeDamage();
        //ComeApart();

        hopDelay = CalculateHopDelay();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case ChickenState.IDLE:
                break;
            case ChickenState.WALKING:

                Walk();
                break;
            case ChickenState.HUNTING:
                Vector3 direction = target.transform.position - transform.position;
                direction.y = 0.0f;
                direction.Normalize();
                transform.position = transform.position + (direction * speed * Time.deltaTime);
                //print("(" + direction.x + ", " + direction.y + ", " + direction.z + ") ");
                //state = ChickenState.IDLE;
                break;
            
            
            case ChickenState.PRE_HOP: // timekeeper must be zero
                if (timeKeeper >= hopDelay) {
                    timeKeeper = 0.0f;
                    state = ChickenState.HOPPING;
                    StartHop();
                    break;
                }

                timeKeeper += Time.deltaTime;
                break;
            case ChickenState.HOPPING:
                Hop();
                break;
            case ChickenState.PRE_ATTACK:
                if (timeKeeper == 0.0f) {
                    breakParts[0].GetComponent<MeshRenderer>().material = attackMaterial;
                }
                else if (timeKeeper >= hopDelay) {
                    timeKeeper = 0.0f;
                    state = ChickenState.ATTACKING;
                    StartAttack();
                    break;
                }

                timeKeeper += Time.deltaTime;
                break;
            case ChickenState.ATTACKING:
                Attack();
                break;
        }

    }

    void StartWalking()
    {
        //stateTimer = 0.0f;
        //state = ChickenState.WALKING;
        //currentVel = transform.rotation * vel;
        ////StartCoroutine(walk animation)
        ///

    }

    void StartHunting()
    {
        state = ChickenState.HUNTING;
    }

    public void TakeDamage()
    {
        print("TakeDamage");
        state = ChickenState.TAKE_DAMAGE;
        transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));

        //ComeApart();

        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.constraints = RigidbodyConstraints.None;

        breakParts[0].GetComponent<MeshRenderer>().material = regularMaterial;

        StartCoroutine(DamageReaction(1.0f));
        
        timeKeeper = 0.0f;
    }

    IEnumerator DamageReaction(float delay)
    {
        ComeApart();
        yield return new WaitForEndOfFrame();
        BlowApart();
    }

    
    
    
    void Walk()
    {
        //transform.position = transform.position + (currentVel * speed * Time.deltaTime);

        //stateTimer += Time.deltaTime;
        //transform.Rotate(new Vector3(0.0f, 15.0f*Time.deltaTime*1.5f, 0.0f));
        //currentVel = transform.rotation * vel;
        
    }


    void ComeApart()
    {
        //GetComponent<CapsuleCollider>().enabled = false;
        foreach(GameObject bodyPart in breakParts) {
            Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            bodyPart.transform.parent = null;
        }
    }

    void BlowApart()
    {
        foreach (GameObject bodyPart in breakParts) {
            Vector3 radialForce = bodyPart.transform.position - transform.position;
            radialForce.y = 0.0f;
            radialForce.Normalize();

            Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
            rb.AddForce((Vector3.up * 2.0f) + (radialForce * 0.5f), ForceMode.Impulse);
            //rb.AddRelativeTorque(new Vector3(22.0f, 0.0f, 0.0f));
            Destroy(bodyPart, Random.Range(3.0f, 7.0f));
        }

        Destroy(gameObject, 5.0f);
    }

    float timeKeeper = 0.0f;
    void StartHop()
    {
        hopStartPos = transform.position;
        distance = 0.0f;
        targetDirection = target.transform.position - transform.position;
        targetDirection.Normalize();
        hopDelay = CalculateHopDelay();
        transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z)); // look at target, but don't look up or down
    }

    public void Hop()
    {
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.isKinematic = false;
        //rb.AddForce(new Vector3(5.0f, 3.0f, 0.0f), ForceMode.Impulse);

        if (distance == 1.0f) {
            // 20% of attacking (maybe increase this when close to target)
            if (Random.value > 0.8f) {
                state = ChickenState.PRE_HOP;
            }
            else {
                state = ChickenState.PRE_ATTACK;
            }
        }
        else {
            float y = -1.0f * (distance - 0.5f) * (distance - 0.5f) + 0.25f; // y = -1*(x-0.5)^2+.25
            Vector3 hopVector = distance * targetDirection;
            hopVector.y = y;

            transform.position = hopStartPos + hopVector;

            distance += (speed * Time.deltaTime);
            if (distance > 1.0f) distance = 1.0f;

            LandOnGround();
        }
    }

    void StartAttack()
    {
        hopStartPos = transform.position;
        distance = 0.0f;
        targetDirection = target.transform.position - transform.position;
        targetDirection.Normalize();
        hopDelay = CalculateHopDelay();
    }

    void Attack()
    {
        if (distance == 3.0f) { // attack is finished
            state = ChickenState.PRE_HOP;
            breakParts[0].GetComponent<MeshRenderer>().material = regularMaterial;
        }
        else {
            float y = -1.0f/2.25f * (distance - 1.5f) * (distance - 1.5f) + 1.0f; // y = -1/2.25 * (x-1.5)^2 + 1
            Vector3 hopVector = distance * targetDirection;
            hopVector.y = y;

            transform.position = hopStartPos + hopVector;

            distance += (1.5f * speed * Time.deltaTime);
            if (distance > 3.0f) distance = 3.0f;

            LandOnGround();
        }
    }

    float CalculateHopDelay()
    {
        return Random.Range(1.0f, 3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            //print("Kill!");
        }
    }

    public bool IsTakingDamage()
    {
        return state == ChickenState.TAKE_DAMAGE;
    }

    void LandOnGround()
    {
        // check for ground while hopping/attacking
        RaycastHit[] hitInfoList;
        Vector3 raycastOrigin = new Vector3(0.0f, 0.0f, 0.4f);
        raycastOrigin = transform.TransformPoint(raycastOrigin);


        //Physics.Raycast(raycastOrigin, Vector3.forward, out hitInfo, 0.2f);
        hitInfoList = Physics.RaycastAll(raycastOrigin, Vector3.down, 0.2f);


        foreach (RaycastHit hitInfo in hitInfoList) {
            if (hitInfo.transform.name == "Terrain") {
                print(hitInfo.transform.name);
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
                print("changed y");
                break;
            }
        }
    }
}
