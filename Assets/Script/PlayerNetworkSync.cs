using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkSync : NetworkBehaviour {

    [SyncVar] private Vector3 syncPos;
    [SyncVar] private Quaternion syncRot;

    [SerializeField] private float lerpRate = 55;

    private void Start() {
        if (isLocalPlayer) {
            GameObject.Find("CameraTarget").SetActive(false);
            GetComponent<PlayerController>().enabled = true;
            transform.Find("PlayerCamera").GetComponent<Camera>().enabled = true;
        }
    }

    private void FixedUpdate() {
        TransmitPosition();
        LerpPosition();
    }

    void LerpPosition() {
        if (!isLocalPlayer) {
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdProvidePositionToServer (Vector3 pos, Quaternion rot) {
        syncPos = pos;
        syncRot = rot;
    }

    [ClientCallback]
    void TransmitPosition() {
        if (isLocalPlayer) {
            CmdProvidePositionToServer(transform.position, transform.rotation);
        }
    }
}
