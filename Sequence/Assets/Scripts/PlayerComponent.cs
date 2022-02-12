using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public float maxSidewaysSpeed = 10f;
    public float jumpStrength = 10f;
    public Utils.GameColor color = Utils.GameColor.White;
    public Utils.Gravity initialGravity = Utils.Gravity.Down;
    public Renderer playerRenderer;

    float sidewaysInput = 0.0f;
    bool mIsGrounded = true;    // TODO change
    bool jump = false;
    float gravityPull;
    Rigidbody mRigidBody;
    Utils utils;

    void Start()
    {
        mRigidBody = GetComponent<Rigidbody>();
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        utils = gameLogic.GetComponent<Utils>();

        UpdateRendererColor();
        ApplyInitialGravity();
    }

    private void OnValidate()
    {
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        if (gameLogic)
        {
            utils = gameLogic.GetComponent<Utils>();
            UpdateRendererColor();
        }
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
            sidewaysInput = -Input.GetAxis("Horizontal");
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

        Vector3 groundDirectionPositive = new Vector3(Mathf.Abs(groundDirection.x), Mathf.Abs(groundDirection.y), 0.0f);
        Vector3 right = Vector3.Cross(Vector3.forward, groundDirectionPositive);
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

    public void SetColor(Utils.GameColor newColor)
    {
        color = newColor;
        UpdateRendererColor();
    }

    void UpdateRendererColor()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        playerRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", utils.GetColor(color));
        playerRenderer.SetPropertyBlock(propBlock);
    }

    void ApplyInitialGravity()
    {
        float gravityPull = Physics.gravity.magnitude;

        switch (initialGravity)
        {
            case Utils.Gravity.Up:
                Physics.gravity = Vector3.up * gravityPull;
                break;
            case Utils.Gravity.Down:
                Physics.gravity = Vector3.down * gravityPull;
                break;
            case Utils.Gravity.Left:
                Physics.gravity = Vector3.left * gravityPull;
                break;
            case Utils.Gravity.Right:
                Physics.gravity = Vector3.right * gravityPull;
                break;
        }
    }

}
