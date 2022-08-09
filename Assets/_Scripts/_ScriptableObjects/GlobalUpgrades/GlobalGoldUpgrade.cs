using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global/Global Gold Upgrade")]
public class GlobalGoldUpgrade : GlobalUpgrade
{
    public int Value;

    public override void Initialize()
    {
        RunManager.Instance.ChangeGoldValue(Value);
    }
}
