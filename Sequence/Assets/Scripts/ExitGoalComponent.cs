using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGoalComponent : GoalComponent
{
    override protected void TriggerGoal()
    {
        flow.OnExitGame();
    }
}
