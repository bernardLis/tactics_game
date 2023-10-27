using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleTileWolfLair : BattleTile
{
    public override void EnableTile()
    {
        base.EnableTile();

        Building wolfLair = Instantiate(_gameManager.GameDatabase.GetBuildingByName("Wolf Lair"));
        wolfLair.Initialize();
        _battleBuilding.Initialize(wolfLair);
    }
}
