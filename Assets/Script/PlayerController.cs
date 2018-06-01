using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour {

    public float moveSpeed = 4f;
    public float jumpForce = 15f;
    public float rotationSpeed = 55f;
    public LayerMask groundLayers;

    private Vector3 forward, right, altRight, up;
    private Rigidbody rb;
    private CapsuleCollider cc;
    private bool isClimbing = false;

	void Start () {
        forward = GameObject.Find("PlayerCamera").transform.forward;// new Vector3(0.6f, -0.5f, 0.6f);//Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        up = GameObject.Find("PlayerCamera").transform.up;// new Vector3(0.6f, -0.5f, 0.6f);//Camera.main.transform.forward;
        up.x = up.z = 0;
        up = Vector3.Normalize(up);

        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
    }
	
	void Update () {
        if (Input.anyKey && isLocalPlayer == true) {
            if (!isClimbing) {
                Move();
            } else {
                Climb();
            }
        }
    }

    void FixedUpdate() {
        initClimb();
        if (Input.anyKey && isLocalPlayer == true) {
            Jump();
        }
    }

    void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            /*if (contact.otherCollider.gameObject.name.Contains("Cube")) {
                if (contact.otherCollider.gameObject.GetComponent<EnvironmentObject>().walkOnWithoutJump && (transform.position.y >= contact.otherCollider.transform.position.y) && IsGrounded()) {
                    transform.position += new Vector3(test.x / 4, contact.otherCollider.gameObject.transform.position.y, test.z / 4);
                }
            }*/
            


            /*if (contact.otherCollider.gameObject.name.Contains("Cube") && contact.otherCollider.gameObject.GetComponent<Collider>().bounds.size.y <= 1) {
                
                float heightDiff = transform.position.y - contact.otherCollider.gameObject.transform.position.y;
                heightDiff = Mathf.Round(heightDiff * 10f) / 10f;
                Debug.Log("THIS: " + contact.thisCollider.transform.position.y + " OTHER: " + contact.otherCollider.gameObject.name + "," + contact.otherCollider.transform.position.y + "DIFF: " + heightDiff);

                if (heightDiff <= 0.5 && heightDiff >= 0) {
                    transform.position += new Vector3(test.x / 2, 1, test.z/2);
                }
            }*/
        }


        /*GameObject temp = collision.gameObject;
        if (temp.name == "Cube" && temp.GetComponent<Collider>().bounds.size.y <= 1) {
            Debug.Log("Collision y: " + temp.transform.position.y + " ME y: " + transform.position.y + " UP: " + temp.transform.up);
            if (transform.position.y <= temp.transform.position.y + 1) {
                transform.position += new Vector3(0, 1, 0);
            }
        }*/
    }

    void Move() {     
        //Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        if (heading != Vector3.zero) { //Prevent Vector3 = 0 error when jumping
            transform.forward += heading * Time.deltaTime * rotationSpeed;
            transform.position += rightMovement;
            transform.position += upMovement;
        }        
    }

    void initClimb() {
        Ray ray = new Ray(transform.position, transform.forward);
        Ray rayL = new Ray(transform.position + new Vector3(-0.5f, 0, 0), transform.forward);
        Ray rayR = new Ray(transform.position + new Vector3(0.5f, 0, 0), transform.forward);

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward, Color.green);
        Debug.DrawRay(transform.position - transform.TransformVector(Vector3.left * 0.5f), transform.forward, Color.red);
        Debug.DrawRay(transform.position - transform.TransformVector(Vector3.left * -0.5f), transform.forward, Color.blue);

        if (isClimbing) {
            rb.drag = 100;
        } else if (!isClimbing) {
            rb.drag = 0;
            //transform.position += Vector3.up * moveSpeed * Time.deltaTime * 10; //temp solution
        }
        
        if (Physics.Raycast(ray, out hit, 0.6f)) {
            if (hit.transform.gameObject.GetComponent<EnvironmentObject>().isClimbable && isClimbing == false) {
                isClimbing = true;
                if (Physics.Raycast(rayL, out hit, 0.6f)) {
                    Debug.Log("TURN LEFT");
                    transform.eulerAngles -= new Vector3(0, 45, 0);
                } else if (Physics.Raycast(rayR, out hit, 0.6f)) {
                    transform.eulerAngles += new Vector3(0, 45, 0);
                }
            }
        } else {
            isClimbing = false;
        }
    }

    void Climb() {
        /*if(Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }*/
        altRight = Quaternion.Euler(new Vector3(0, 90, 0)) * transform.forward;
        Vector3 rightMovement = altRight * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = up * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        if (heading != Vector3.zero) { //Prevent Vector3 = 0 error when jumping
            //transform.forward += heading * Time.deltaTime * rotationSpeed;
            transform.position += rightMovement;
            transform.position += upMovement;
        }

        //Jump off ladder (temp solution)
        if (Input.GetKey(KeyCode.Space)) {
            transform.position += -Vector3.forward * moveSpeed * Time.deltaTime * 10;
        }
    }

    void Jump() {
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        } /*else if (IsGrounded() == false && isClimbing) {
            rb.isKinematic = false;
            isClimbing = false;
        }*/
    }

    private bool IsGrounded() {
        //Added rb.velocity.y >= -1 to prevent capsule collider to spasm on edges and allow jumping on edge/ground
        return Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y, cc.bounds.center.z), cc.radius * .9f, groundLayers) && rb.velocity.y >= -1;
    }

    public bool ClimbingEnabled {
        get {
            return isClimbing;
        }
        set {
            isClimbing = value;
        }
    }
}
