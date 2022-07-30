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
    PauseController pauseController;
    //BoxCollider attackBox;
    bool isAttacking = false;
    Vector3 velocity;
    int score = 0;
    HitPoints hp;
    float stickX, stickZ;
    float targetDirection = 0.0f;



    private void Awake()
    {
        //attackBox = GetComponent<BoxCollider>();
        Physics.IgnoreLayerCollision(3, 3);
        Physics.IgnoreLayerCollision(3, 6);
        Physics.IgnoreLayerCollision(7, 7);
        Physics.IgnoreLayerCollision(7, 8);

        velocity = Vector3.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        pauseController = GetComponent<PauseController>();

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
        if (pauseController.IsPaused()) return;

        if (transform.position.y < -15) {
            transform.position = new Vector3(0, 10, 0);
        }

        if (Input.GetMouseButtonDown(0) && !animator.GetBool("SwingSword")) {
            animator.SetTrigger("SwingSword");
        }

        //// Gamepad
        UpdateController();


        // WASD Controls
        if (Input.GetKeyUp(Global.UP_KEY) || Input.GetKeyUp(Global.DOWN_KEY) || (ForwardInput() && BackwardInput())) {
            velocity.z = 0.0f;
        }
        else if (ForwardInput()) {
            velocity.z += acceleration;// * Time.deltaTime;
            if (velocity.z > speed) velocity.z = speed;

            stickZ = 1;
        }

        if (BackwardInput()) {
            velocity.z -= acceleration;// * Time.deltaTime;
            if (velocity.z < -speed) velocity.z = -speed;

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

                stickX = 1;
            }

            if (LeftInput()) {
                velocity.x -= acceleration;// * Time.deltaTime;
                if (velocity.x < -speed) velocity.x = -speed;

                stickX = -1;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) {

            animator.SetBool("KnockedDown2", true);
            StartCoroutine(DieRagdoll());
        }

       

        if (Input.GetKeyDown(KeyCode.Space)) {
            KnockDown();
        }

        if (Input.GetKeyDown(KeyCode.Z)) {

            hp.Decrement(5);

        }

        if (Input.GetKeyDown(KeyCode.I)) {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            animator.SetTrigger("Get Hit");
            print("Get Hit animation");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        float yy = rb.velocity.y;
        rb.velocity = new Vector3(stickX * speed2, yy, stickZ * speed2);
        rb.AddForce(Vector3.down * 20.0f); // gravity adjustment (THIS IS GREAT!!)
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude / speed2));

        if (stickX + stickZ != 0) {
            float yAngle = Mathf.Atan2(stickX, stickZ) * 180.0f / Mathf.PI; // this will change to targetAngle

            if (yAngle < 0) yAngle += 360;
            targetDirection = yAngle;
        }

        //RotateTowardTarget(targetDirection, turnSpeed);
        RotateTowardTarget();

        //transform.Rotate(new Vector3(0.0f, 30.0f * Time.deltaTime, 0.0f));
        //if (Time.time - tk > 0.5f) {
        //    tk = Time.time;

        //    float eulerY = transform.rotation.eulerAngles.y;
        //    //if (eulerY > 180.0f) {
        //    //    eulerY = eulerY;
        //    //}
        //    //print("Current Y Angle: "+eulerY);
        //    print("Current Target Dir: "+targetDirection);
        //}

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


        float targetY = (targetYRot + 360) % 360.0f; // remove negative values and change to [0,360)
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        float eulerY = eulerAngles.y;
        float diff = eulerY - targetY;

        float degrees = degPerSec * Time.deltaTime;

        print("eulerY: " + eulerY + " targetYRot: " + targetYRot + " diff: " + diff);

        // check size of diff here and adjust
        if (Mathf.Abs(diff) < degrees) {
            transform.rotation = Quaternion.Euler(0.0f, targetY, 0.0f);
            //print("Direct angle assignment");
            return;
        }

        if (diff < 0) {
            transform.Rotate(new Vector3(0.0f, degrees, 0.0f));
        }
        else {
            transform.Rotate(new Vector3(0.0f, -degrees, 0.0f));
        }
    }

    void RotateTowardTarget() // assumes that angles are [0, 360)
    {
        //print("Target: " + targetDirection + " Current: " + (transform.rotation.eulerAngles.y-0.0f));
        float diff = targetDirection - transform.rotation.eulerAngles.y;
        float degreesThisFrame = turnSpeed * Time.deltaTime;

        if (Mathf.Abs(diff) <= degreesThisFrame) {
            //RotateImmediately(targetDirection);
            return;
        }

       // There's probably a better way to do this
        if (diff > 0.0f) {
            if (diff < 180.0f)
                transform.Rotate(new Vector3(0.0f, 360.0f * Time.deltaTime, 0.0f)); // positive and under 180 (+)
            else
                transform.Rotate(new Vector3(0.0f, -360.0f * Time.deltaTime, 0.0f)); // positive and over 180 (-)
        }
        else {
            if (diff < -180.0f)
                transform.Rotate(new Vector3(0.0f, 360.0f * Time.deltaTime, 0.0f)); // negative and under -180 (+)
            else
                transform.Rotate(new Vector3(0.0f, -360.0f * Time.deltaTime, 0.0f)); // negative and over -180 (-)
        }
    }

    void RotateImmediately(float yRot)
    {
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0.0f, yRot, 0.0f);
        transform.rotation = rotation;
    }
}
