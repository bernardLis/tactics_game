using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Troops Limit")]
public class CampBuildingTroopsLimit : CampBuilding
{
    public int LimitIncrease1;
    public int LimitIncrease2;
    public int LimitIncrease3;
    public int LimitIncrease4;


    public override void FinishBuilding()
    {
        base.FinishBuilding();
        _gameManager.ChangeTroopsLimit(LimitIncrease1);
    }

    public override void Upgrade()
    {
        base.Upgrade();
        // TODO: obviously something nicer
        if (UpgradeLevel == 2)
            _gameManager.ChangeTroopsLimit(LimitIncrease2);
        if (UpgradeLevel == 3)
            _gameManager.ChangeTroopsLimit(LimitIncrease3);
        if (UpgradeLevel == 4)
            _gameManager.ChangeTroopsLimit(LimitIncrease4);

    }

}
