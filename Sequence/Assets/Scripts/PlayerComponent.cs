using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public float maxSidewaysSpeed = 10f;
    public float maxVerticalSpeed = 10f;
    public float jumpStrength = 10f;
    public Utils.GameColor color = Utils.GameColor.White;
    public Utils.Gravity initialGravity = Utils.Gravity.Down;
    public float onSubBeatScale = 0.85f;
    public float onMainBeatScale = 1.2f;
    public float onBeatAnimationSpeed = 20.0f;
    public Renderer playerRenderer;
    public GameObject meshObject;

    float sidewaysInput = 0.0f;
    bool mIsGrounded = true;
    bool jump = false;
    float gravityPull;
    Rigidbody mRigidBody;
    Utils utils;
    float animationProgress = 0.0f;
    bool animationStart = true;

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

        Vector3 groundDirection = Physics.gravity.normalized;
        int layerMask = 1 << 3;
        mIsGrounded = Physics.Raycast(transform.position, groundDirection, transform.localScale.x + 0.1f, layerMask);

        if (Input.GetButtonDown("Jump") && mIsGrounded)
        {
            jump = true;
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
            int layerMask = 1 << 3;
            RaycastHit hitInfo;
            bool sidewaysHit = Physics.Raycast(transform.position, (right * sidewaysInput).normalized, out hitInfo, transform.localScale.x + 0.01f, layerMask);
            
            if (sidewaysHit)
            {
                // Exclude walls with the same color
                WallComponent wall = hitInfo.transform.gameObject.GetComponent<WallComponent>();
                if (wall)
                {
                    if (wall.color == color)
                    {
                        sidewaysHit = false;
                    }
                }
            }

            if (!sidewaysHit)
            {
                if (sidewaysInput < 0.0f && currentSidewaysSpeed > -maxSidewaysSpeed
                 || sidewaysInput > 0.0f && currentSidewaysSpeed < maxSidewaysSpeed)
                {
                    mRigidBody.velocity += right * sidewaysInput * maxSidewaysSpeed;
                }
            }
        }
        else
        {
            if (Mathf.Abs(currentSidewaysSpeed) != 0.0f)
            {
                mRigidBody.velocity -= right * Mathf.Min(maxSidewaysSpeed * 0.2f, currentSidewaysSpeed);
            }
        }

        float currentVerticalSpeed = mRigidBody.velocity.x * -groundDirection.x + mRigidBody.velocity.y * -groundDirection.y;
        if (Mathf.Abs(currentVerticalSpeed) > maxVerticalSpeed)
        {
            mRigidBody.velocity -= groundDirection * (Mathf.Abs(currentVerticalSpeed) - maxVerticalSpeed);
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

    public void OnAudioBeat(bool isMainBeat)
    {
        StartCoroutine(AudioBeatAnimation(isMainBeat));
    }

    IEnumerator AudioBeatAnimation(bool isMainBeat)
    {
        bool animationDone = false;
        Vector3 previousScale = transform.localScale;

        while (!animationDone)
        {
            if (animationStart)
            {
                animationProgress += onBeatAnimationSpeed * Time.deltaTime;
                if (animationProgress > 1.0f)
                {
                    animationProgress = 1.0f;
                    animationStart = false;
                }
            }
            else
            {
                animationProgress -= onBeatAnimationSpeed * Time.deltaTime;
                if (animationProgress < 0.0f)
                {
                    animationProgress = 0.0f;
                    animationStart = true;
                    animationDone = true;
                }
            }

            float multiplier;
            if (isMainBeat)
            {
                multiplier = onMainBeatScale;
            }
            else
            {
                multiplier = onSubBeatScale;
            }

            meshObject.transform.localScale = Vector3.Lerp(previousScale, previousScale * multiplier, animationProgress);

            yield return null;
        }
    }
}
