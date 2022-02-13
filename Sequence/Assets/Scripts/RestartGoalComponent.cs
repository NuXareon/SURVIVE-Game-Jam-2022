using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGoalComponent : GoalComponent
{
    override protected void TriggerGoal()
    {
        flow.OnCompleteGame();
    }
}
