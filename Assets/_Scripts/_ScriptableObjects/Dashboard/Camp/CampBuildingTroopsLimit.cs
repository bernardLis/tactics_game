using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Troops Limit")]
public class CampBuildingTroopsLimit : CampBuilding
{
    public int LimitIncrease;

    public override void FinishBuilding()
    {
        base.FinishBuilding();
        _gameManager.ChangeTroopsLimit(LimitIncrease);
    }

}
