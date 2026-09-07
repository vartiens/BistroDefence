using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private InputActionReference fireActionReference;
    [SerializeField] private float jumpForce = 500.0f;

    //private XROrigin _xrOrigin;
    //private CapsuleCollider _collider;
    private Rigidbody _body;
    //private bool isGrounded => Physics.Raycast(new Vector2(transform.position.x, transform.position.y + 2.0f), Vector3.down, 2.0f);
    private bool isGrounded = true;
    void Start() {
        //_xrOrigin = GetComponent<XROrigin>();
        //_collider = GetComponent<CapsuleCollider>();
        _body = GetComponent<Rigidbody>();
        jumpActionReference.action.performed += Onjump;
        jumpActionReference.action.performed += Onfire;
        isGrounded = true;
    }
    /*
    private void Update() {
        var center = _xrOrigin.CameraInOriginSpacePos;
        _collider.center = new Vector3(center.x, _collider.center.y, center.z);
        _collider.height = _xrOrigin.CameraInOriginSpaceHeight;
    }
    */
    private void Onjump(InputAction.CallbackContext obj) {
        if (!isGrounded)
            return;
        _body.AddForce(Vector3.up * jumpForce);
        isGrounded = false;
    }

    private void Onfire(InputAction.CallbackContext obj) {
        
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Grounded")
            isGrounded = true;
    }
}
