using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour {

    public float moveSpeed = 4f;
    public float jumpForce = 15f;
    public float rotationSpeed = 55f;
    public LayerMask groundLayers;

    private Vector3 forward, right;
    private Rigidbody rb;
    private CapsuleCollider cc;

	// Use this for initialization
	void Start () {
        forward = GameObject.Find("PlayerCamera").transform.forward;// new Vector3(0.6f, -0.5f, 0.6f);//Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey && isLocalPlayer == true) {
            Move();
            Jump();
        }        
    }

    private void FixedUpdate() {
                
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

    void Jump() {
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded() {
        return Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y, cc.bounds.center.z), cc.radius * .9f, groundLayers);
    }
}
