using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    public Transform target;
    public Vector3 cameraRotation;
    public Vector3 offset;

    private void LateUpdate() {
        transform.rotation = Quaternion.Euler(cameraRotation);
        transform.position = target.position + offset;
    }
}
