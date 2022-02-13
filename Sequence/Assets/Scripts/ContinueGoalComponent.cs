using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGoalComponent : GoalComponent
{
    override protected void TriggerGoal()
    {
        flow.OnContinueGame();
    }
}
