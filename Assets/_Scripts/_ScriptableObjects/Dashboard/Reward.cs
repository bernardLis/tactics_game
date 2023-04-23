using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward")]
public class Reward : BaseScriptableObject
{
    protected GameManager _gameManager;

    protected Hero _hero;

    public event Action<Reward> OnRewardSelected;
    public virtual void CreateRandom(Hero hero)
    {
        _gameManager = GameManager.Instance;
        _hero = hero;
    }

    public virtual void GetReward()
    {
        OnRewardSelected?.Invoke(this);
    }
}
