using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField][Range(0.1f,0.1f)] float speed;
    [SerializeField] [Range(0.1f, 0.5f)] float acceleration;
    //[SerializeField] GameObject weapon;
    [SerializeField] float knockedDownTime = 1.5f;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] Text scoreboard;
    [SerializeField] GameObject[] bodyParts;
    [SerializeField] GameObject shovel;
    [SerializeField] Image lifeBar;
    [SerializeField] [Range(1, 20)] float speed2;
    [SerializeField] float turnSpeed;

    Animator animator;
    BoxCollider attackBox;
    bool isAttacking = false;
    Vector3 velocity;
    int score = 0;
    //int life = 100;
    HitPoints hp;
    float stickX, stickZ;
    const float ROOT_3 = 1.7320508075688772935274463415059f;
    float targetDirection = 180.0f;



    private void Awake()
    {
        attackBox = GetComponent<BoxCollider>();
        Physics.IgnoreLayerCollision(3, 3);
        Physics.IgnoreLayerCollision(3, 6);
        Physics.IgnoreLayerCollision(7, 7);
        Physics.IgnoreLayerCollision(7, 8);
        //Physics.IgnoreLayerCollision(6, 7);
        //Physics.IgnoreLayerCollision(6, 6);
        velocity = Vector3.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //StartCoroutine(RaycastCoroutine());
        StartCoroutine(TestController());

        // Set up HitPoint component
        hp = GetComponent<HitPoints>();
        hp.onZeroOrLess += Die;// KnockdownAnimation;
        hp.onChange += UpdateLifeBar;
        hp.Set(100);

        stickX = stickZ = 0.0f;

        //print("atan checks");
        //print("1) " + Mathf.Atan2(1.0f, ROOT_3) * Mathf.Rad2Deg);
        //print("2) " + Mathf.Atan2(1.0f, -ROOT_3) * Mathf.Rad2Deg);
        //print("3) " + Mathf.Atan2(-1.0f, -ROOT_3) * Mathf.Rad2Deg);
        //print("4) " + Mathf.Atan2(-1.0f, ROOT_3) * Mathf.Rad2Deg);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15) {
            transform.position = new Vector3(0, 10, 0);
        }

        if (Input.GetMouseButtonDown(0) && !animator.GetBool("SwingSword")) {
            animator.SetTrigger("SwingSword");
            //isAttacking = true;
        }

        UpdateController();

        // Tank Controls
        //Vector3 direction = transform.rotation * Vector3.forward;
        //transform.position = transform.position + (direction * (speed * 10) * Time.deltaTime);

        //if (ForwardInput()) { // Forward
        //    speed += 1.0f * Time.deltaTime;
        //    if (speed > 0.5f) speed = 0.5f;
        //}
        ////else if (BackwardInput()) { // Backward
        ////    speed -= 1.0f * Time.deltaTime;
        ////    if (speed < -0.5f) speed = -0.5f;
        ////}
        //else if (BackwardInput()) { // Backward
        //    transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        //}
        //else {
        //    speed = 0.9f * speed;
        //}

        //// Rotation
        //if (LeftInput()) {
        //    transform.Rotate(new Vector3(0.0f, -180.0f * Time.deltaTime, 0.0f));
        //}
        //if (RightInput()) {
        //    transform.Rotate(new Vector3(0.0f, 180.0f * Time.deltaTime, 0.0f));
        //}

        // Direct Controls

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || (ForwardInput() && BackwardInput())) {
            velocity.z = 0.0f;
        }
        else if (ForwardInput()) {
            velocity.z += acceleration;// * Time.deltaTime;
            if (velocity.z > speed) velocity.z = speed;

            //Quaternion rotation = new Quaternion();
            //rotation.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            //transform.rotation = rotation;

            //RotateImmediately(0.0f);

            stickZ = 1;
        }

        if (BackwardInput()) {
            velocity.z -= acceleration;// * Time.deltaTime;
            if (velocity.z < -speed) velocity.z = -speed;

            //Quaternion rotation = new Quaternion();
            //rotation.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            //transform.rotation = rotation;

            //RotateImmediately(180.0f);

            stickZ = -1;
        }



        // If you release a key or push both at once, stop
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || (LeftInput() && RightInput())) {
            velocity.x = 0.0f;
        }
        else {
            if (RightInput()) {
                velocity.x += acceleration;// * Time.deltaTime;
                if (velocity.x > speed) velocity.x = speed;

                //Quaternion rotation = new Quaternion();
                //rotation.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                //transform.rotation = rotation;

                //RotateImmediately(90.0f);

                stickX = 1;
            }

            if (LeftInput()) {
                velocity.x -= acceleration;// * Time.deltaTime;
                if (velocity.x < -speed) velocity.x = -speed;

                //Quaternion rotation = new Quaternion();
                //rotation.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
                //transform.rotation = rotation;

                //RotateImmediately(-90.0f);

                stickX = -1;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) {

            animator.SetBool("KnockedDown2", true);
            StartCoroutine(DieRagdoll());
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        //transform.position = transform.position+velocity;
        Vector3 tempVel = rb.velocity;
        tempVel.x = velocity.x * 100;
        tempVel.z = velocity.z * 100;
        tempVel.y -= 0.1f; // gravity adjustment (THIS IS GREAT!!)
        rb.velocity = tempVel;
        //rb.AddForce(100*velocity, ForceMode.Impulse);

        if (Input.GetKeyDown(KeyCode.Space)) {
            //GameObject chicken = GameObject.FindGameObjectWithTag("Chicken");
            //chicken.GetComponent<ChickenController>().Hop();
            KnockDown();
            //Vector3 pos = transform.position;
            //pos.y = 10.0f;
            //transform.position = pos;
        }

        //animator.SetFloat("Speed", Mathf.Abs(velocity.magnitude * 10.0f));

        if (Input.GetKeyDown(KeyCode.Z)) {

            hp.Decrement(5);

        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            animator.SetTrigger("Get Hit");
            print("Get Hit animation");
        }

        float yy = rb.velocity.y;
        rb.velocity = new Vector3(stickX*speed2, yy, stickZ*speed2);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude / speed2));

        if (stickX + stickZ != 0) {
            //if (stickX > 0 && stickZ > 0) {

            //}
            float yAngle = Mathf.Atan2(stickX, stickZ) * 180.0f / Mathf.PI; // this will change to targetAngle
            //Quaternion rotation = new Quaternion();
            //rotation.eulerAngles = new Vector3(0, yAngle, 0);
            //transform.rotation = rotation;

            targetDirection = yAngle + 180.0f;
        }

        RotateTowardTarget(targetDirection, turnSpeed);
 
    } // end Update

    void KnockdownAnimation()
    {
        animator.SetBool("KnockedDown2", true);
        //print("Knocked Down");
    }

    public void Hit()
    {
        //print("Hit()");
        //attackBox.enabled = true;
        isAttacking = true;
        animator.ResetTrigger("SwingSword");
    }

    public void EndSwing()
    {
        //print("EndSwing()");
        isAttacking = false;
        //attackBox.enabled = false;
    }

   
    bool ForwardInput()
    {
        return Input.GetKey(KeyCode.W);// || Input.GetKey(KeyCode.UpArrow);
    }

    bool BackwardInput()
    {
        return Input.GetKey(KeyCode.S);// || Input.GetKey(KeyCode.DownArrow);
    }

    bool LeftInput()
    {
        return Input.GetKey(KeyCode.A);// || Input.GetKey(KeyCode.LeftArrow);
    }

    bool RightInput()
    {
        return Input.GetKey(KeyCode.D);// || Input.GetKey(KeyCode.RightArrow);
    }

    public void KnockDown()
    {
        animator.SetBool("KnockedDown2", true);
        StartCoroutine(GetUp());
    }

    IEnumerator GetUp()
    {
        //transform.position = transform.position + new Vector3(0, 1f, 0);
        yield return new WaitForSeconds(knockedDownTime);

        animator.SetBool("KnockedDown2", false);

        //foreach (GameObject bodyPart in bodyParts) {
        //    bodyPart.GetComponent<Rigidbody>().isKinematic = false;
        //    bodyPart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //}
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore()
    {
        ++score;
    }

    public void AddScore(int increase)
    {
        score += increase;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    float CastRayFromFeet()
    {
        RaycastHit[] hitInfoList = Physics.RaycastAll(transform.position+new Vector3(0,1.83f,0), Vector3.forward, 1.0f);

        foreach(RaycastHit hitInfo in hitInfoList) {
            if (hitInfo.transform.tag == "Ground") {
                print(Vector3.Dot(hitInfo.normal, Vector3.forward));
                return Vector3.Dot(hitInfo.normal, Vector3.forward);
            }
        }

        return 0.0f;
    }

    IEnumerator RaycastCoroutine()
    {
        while (true) {
            CastRayFromFeet();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator TestController()
    {
        Text horizontalText = GameObject.Find("Horizontal Test").GetComponent<Text>();
        Text verticalText = GameObject.Find("Vertical Test").GetComponent<Text>();

        while (true) {
            yield return new WaitForSeconds(0.2f);

            float horiz = stickX;// Input.GetAxis("Horizontal");
            float vert = stickZ;// Input.GetAxis("Vertical");

            //if (Mathf.Abs(horiz) < 0.4f) horiz = 0.0f;
            //if (Mathf.Abs(vert) < 0.4f) vert = 0.0f;

            horizontalText.text = "Horizontal: "+horiz;
            verticalText.text = "Vertical: "+vert;
        }
    }

    void UpdateController()
    {
        stickX = Input.GetAxis("Horizontal");
        stickZ = Input.GetAxis("Vertical");
    }

    void RagDoll()
    {
        animator.enabled = false;
        
        transform.position = transform.position + new Vector3(0,2,0);
        //foreach(GameObject part in bodyParts) {
        //    part.GetComponent<Rigidbody>().isKinematic = false;
        //    part.GetComponent<Rigidbody>().useGravity = true;
        //    part.GetComponent<Collider>().enabled = true;
        //    part.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //}
        SetKinematic(transform, false);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        

        shovel.transform.parent = null;
        //Physics.IgnoreLayerCollision(7, 7, false);
    }

    void SetKinematic(Transform t, bool newState)
    {
        for (int i = 0; i < t.childCount; i++) {
            SetKinematic(t.GetChild(i), newState);
        }

        Rigidbody rb = t.GetComponent<Rigidbody>();
        if(rb) {
            rb.isKinematic = newState;
            //t.GetComponent<Collider>().enabled = true;//kludge
            SetColliderEnabled(t.gameObject);
        }
    }

    void SetColliderEnabled(GameObject obj)
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        BoxCollider bc = GetComponent<BoxCollider>();
        CapsuleCollider cc = GetComponent<CapsuleCollider>();

        if(sc) {
            sc.enabled = true;
            sc.isTrigger = false;
        }
        if (bc) {
            bc.enabled = true;
            bc.isTrigger = false;
        }
        if (cc) {
            cc.enabled = true;
            cc.isTrigger = false;
        }
    }

    IEnumerator DieRagdoll()
    {
        yield return new WaitForSeconds(1.0f);
        
        GetComponent<Animator>().enabled = false;
        RagDoll();
        print("Ragdolled!");
    }

    void UpdateLifeBar()
    {
        //lifeBar.rectTransform.sizeDelta = new Vector2(4*life, 25);
        lifeBar.rectTransform.sizeDelta = new Vector2(4*hp.Get(), 25);
    }

    void Die()
    {
        animator.SetBool("Dead", true);
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void OnDamaged()
    {
        animator.SetTrigger("Get Hit");
    }

    public void OnKnockedDown()
    {
        KnockDown();
    }

    void RotateTowardTarget(float targetYRot, float degPerSec)
    {
        float targetYRotMod = targetYRot % 360.0f;
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        float diff = eulerAngles.y - targetYRot;

        // check size of diff here and adjust

        if (diff < 0.0f) diff += 360.0f;

        if (diff < 180) {
            transform.Rotate(new Vector3(0.0f, degPerSec * Time.deltaTime, 0.0f));
        }
        else {
            transform.Rotate(new Vector3(0.0f, -degPerSec * Time.deltaTime, 0.0f));
        }
    }

    void RotateImmediately(float yRot)
    {
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0.0f, yRot, 0.0f);
        transform.rotation = rotation;
    }
}
