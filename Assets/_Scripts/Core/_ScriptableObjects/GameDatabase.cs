using System;
using System.Collections.Generic;
using System.Linq;
using Lis.Battle.Pickup;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Core
{
    public class GameDatabase : BaseScriptableObject
    {
        [Header("Battle")] public Battle.Battle SampleBattle;

        [Header("Shaders")] public Shader LitShader;
        public Shader ParticlesUnlitShader;
        public Shader DissolveShader;
        public Shader GrayScaleShader;
        public Shader SepiaToneShader;

        public List<Shader> KeepShadersMaterials = new();

        [Header("Colors")]
        [SerializeField]
        ColorVariable[] _colors;


        [Header("Pickups")]
        [SerializeField]
        Pickup[] _pickups;

        [Header("Icons")]
        public Sprite VaseIcon;

        public Sprite[] LevelUpAnimationSprites;
        public Sprite[] TroopsElementAnimationSprites;

        [FormerlySerializedAs("CoinSprites")] [SerializeField]
        Sprite[] _coinSprites;

        public ColorVariable GetColorByName(string n)
        {
            return _colors.FirstOrDefault(c => c.name == n);
        }

        public Pickup GetPickupByName(string n)
        {
            return _pickups.FirstOrDefault(p => p.name == n);
        }

        public List<Pickup> GetAllPickups()
        {
            return _pickups.ToList();
        }

        public Sprite GetCoinSprite(int amount)
        {
            int index = 0;
            // TODO: something smarter
            if (amount >= 0 && amount <= 100)
                index = 0;
            if (amount >= 101 && amount <= 500)
                index = 1;
            if (amount >= 501 && amount <= 1000)
                index = 2;
            if (amount >= 1001 && amount <= 3000)
                index = 3;
            if (amount >= 3001)
                index = 4;

            return _coinSprites[index];
        }
    }

    public enum StatType
    {
        Power,
        Health,
        Armor,
        Speed,
        AttackRange,
        AttackCooldown,
        Pull,
        None,
        ExpBonus,
        Stamina
    }

    public enum NatureName
    {
        Fire,
        Water,
        Wind,
        Earth,
        Lightning,
        Metal,
        Wood,
        Ice,
        None,
        Neutral
    }

    public enum TargetingType
    {
        Closest,
        Random,
        Strongest,
        Weakest,
    }


    [Serializable]
    public struct StatBasics
    {
        public StatType StatType;
        public Sprite Sprite;
        public string Description;
    }
}