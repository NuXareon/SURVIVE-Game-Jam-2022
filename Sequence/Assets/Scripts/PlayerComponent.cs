using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public float maxSidewaysSpeed = 10f;
    public float jumpStrength = 10f;
    //public float gravityScale = 1.5f;
    //public Camera mainCamera;

    float sidewaysInput = 0.0f;
    bool mIsGrounded = true;    // TODO change
    bool jump = false;
    float gravityPull;
    Rigidbody mRigidBody;

    void Start()
    {
        mRigidBody = GetComponent<Rigidbody>();

        // TODO this probably needs to go to another file
        gravityPull = Physics.gravity.magnitude;
        Physics.gravity = new Vector3(1.0f, 0.0f, 0.0f) * gravityPull;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Dot(Physics.gravity.normalized, Vector3.up) == 0.0f)
        {
            sidewaysInput = Input.GetAxis("Vertical");
        }
        else
        {
            sidewaysInput = Input.GetAxis("Horizontal");
        }


        if (Input.GetButtonDown("Jump") && mIsGrounded)
        {
            jump = true;
            //mRigidBody.AddForce(-groundDirection * jumpStrength, ForceMode.VelocityChange);
            // Jump
        }
        else if (Input.GetButtonUp("Jump"))
        {
            // Stop jump early
            // !check gravity direction
            //velocity.y = velocity.y * 0.5f;
        }
    }

    void FixedUpdate()
    {
        Vector3 groundDirection = Physics.gravity.normalized;
        if (jump)
        {
            // TODO improve jump
            Vector3 jumpSpeed = (-groundDirection * jumpStrength);
            Vector3 newVelocity = mRigidBody.velocity;
            if (jumpSpeed.x != 0.0f)
            {
                newVelocity.x = jumpSpeed.x;
            }
            if (jumpSpeed.y != 0.0f)
            {
                newVelocity.y = jumpSpeed.y;
            }
            mRigidBody.velocity = newVelocity;
            jump = false;
        }

        Vector3 right = Vector3.Cross(Vector3.forward, groundDirection);
        float currentSidewaysSpeed = mRigidBody.velocity.x * right.x + mRigidBody.velocity.y * right.y;
        if (sidewaysInput != 0.0f)
        {
            
            // TODO check for sideways collisions
            if (sidewaysInput < 0.0f && currentSidewaysSpeed > -maxSidewaysSpeed
                || sidewaysInput > 0.0f && currentSidewaysSpeed < maxSidewaysSpeed)
            {
                mRigidBody.velocity += right * sidewaysInput * maxSidewaysSpeed;
            }
            //mRigidBody.AddForce(right * sidewaysSpeed * sidewaysInput);
            // Check current speed so we don't accelerate too much
            //mRigidBody.AddForce(right*sidewaysSpeed*sidewaysInput);
        }
        else
        {
            if (Mathf.Abs(currentSidewaysSpeed) != 0.0f)
            {
                mRigidBody.velocity -= right * Mathf.Min(maxSidewaysSpeed * 0.2f, currentSidewaysSpeed);
            }

            // move towards 0 speed
        }

    }
}
