using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalComponent : MonoBehaviour
{
    protected GameFlow flow;
    GameObject playerObject;
    Vector3 playerLastVelocity;
    bool wasGameEnding = false;
    bool triggeredGameEnding = false;
    public float onSubBeatScale = 0.85f;
    public float onMainBeatScale = 1.2f;
    public float onBeatAnimationSpeed = 20.0f;

    float animationProgress = 0.0f;
    bool animationStart = true;

    void Start()
    {
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        flow = gameLogic.GetComponent<GameFlow>();

        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (flow.gameState == GameFlow.GameState.LevelEnd)
        {
            Rigidbody playerRigidBody = playerObject.GetComponent<Rigidbody>();
            if (!wasGameEnding && triggeredGameEnding)
            {
                playerLastVelocity = playerRigidBody.velocity;

                int KChildIndex = Random.Range(0, transform.childCount);
                transform.GetChild(KChildIndex).gameObject.SetActive(true);

                wasGameEnding = true;
            }

            Vector3 direction = transform.position - playerObject.transform.position;

            if (direction.sqrMagnitude > 0.01f)
            {
                // I'm sure all of this is wrong, but it works sooo...

                float gravityForce = (100.0f / direction.sqrMagnitude);
                gravityForce = Mathf.Min(gravityForce, 100.0f);

                playerLastVelocity += direction.normalized * gravityForce * Time.unscaledDeltaTime;

                // drag
                playerLastVelocity = playerLastVelocity * (1.0f - Time.unscaledDeltaTime * 10.0f);

                playerObject.transform.Translate(playerLastVelocity * Time.unscaledDeltaTime);
            }
            else
            {
                playerObject.transform.position = transform.position;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerGoal();
            triggeredGameEnding = true;
        }
    }

    virtual protected void TriggerGoal()
    {
        flow.OnLevelCompleted();
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

            transform.localScale = Vector3.Lerp(previousScale, previousScale * multiplier, animationProgress);

            yield return null;
        }
    }
}