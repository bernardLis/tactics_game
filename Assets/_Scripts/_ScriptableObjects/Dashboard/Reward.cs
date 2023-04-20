using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward")]
public class Reward : BaseScriptableObject
{
    protected GameManager _gameManager;

    [Header("Static")]
    public Sprite[] ChestIdleSprites;
    public Sprite[] ChestOpenSprites;
    public float UncommonItemChance;
    public float RareItemChance;
    public float EpicItemChance;

    [HideInInspector] public int Gold;
    [HideInInspector] public Item Item;
    [HideInInspector] public int Spice;


    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;

    }

    Item ChooseItem()
    {
        float v = Random.value;
        if (v < EpicItemChance)
            return _gameManager.HeroDatabase.GetRandomEpicItem();
        if (v < RareItemChance)
            return _gameManager.HeroDatabase.GetRandomRareItem();
        if (v < UncommonItemChance)
            return _gameManager.HeroDatabase.GetRandomUncommonItem();

        return _gameManager.HeroDatabase.GetRandomCommonItem();
    }

    public virtual void GetReward()
    {
        if (Gold != 0)
            _gameManager.ChangeGoldValue(Gold);

        if (Spice != 0)
            _gameManager.ChangeSpiceValue(Spice);
    }
}
