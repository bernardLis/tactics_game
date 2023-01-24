using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Camp Building/Better Quests")]
public class CampBuildingBetterQuests : CampBuilding
{
    public override void FinishBuilding() { base.FinishBuilding(); }

    public override void Upgrade()
    {
        base.Upgrade();
        _gameManager.SetMaxQuestRank(_gameManager.MaxQuestRank + 1);
    }
}

