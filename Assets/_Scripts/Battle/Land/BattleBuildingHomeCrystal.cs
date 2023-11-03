using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleBuildingHomeCrystal : BattleBuilding
{

    protected override void ShowBuilding()
    {
        transform.DOMoveY(3, 2f)
                .SetDelay(7f)
                .OnComplete(() =>
                    transform.DORotate(new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360)
                            .SetLoops(-1, LoopType.Restart)
                    );
    }

}
