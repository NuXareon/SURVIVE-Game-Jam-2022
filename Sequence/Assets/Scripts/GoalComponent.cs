using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalComponent : MonoBehaviour
{
    GameFlow flow;
    
    void Start()
    {
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        flow = gameLogic.GetComponent<GameFlow>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flow.OnLevelCompleted();
        }
    }
}
