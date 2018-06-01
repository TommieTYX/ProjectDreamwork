using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour {
    public bool isClimbable = false;
    public bool walkOnWithoutJump = false;

    private void OnCollisionEnter(Collision collision) {
        //Debug.Log("WALL: " + isClimbable);
        if (collision.gameObject.tag == "Player" && isClimbable == true) {
            //collision.gameObject.GetComponent<PlayerController>().ClimbingEnabled = true;
        }
    }
}
