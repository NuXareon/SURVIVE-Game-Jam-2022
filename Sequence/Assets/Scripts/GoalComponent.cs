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
}
