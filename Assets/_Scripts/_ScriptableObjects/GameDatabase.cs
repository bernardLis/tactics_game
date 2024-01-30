using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class GameDatabase : BaseScriptableObject
    {
        [FormerlySerializedAs("Buildings")] [Header("Buildings")] [SerializeField]
        Building[] _buildings;

        public Building GetBuildingByName(string n)
        {
            return _buildings.FirstOrDefault(b => b.name == n);
        }

        public void InitializeBuildings()
        {
            foreach (Building b in _buildings)
                b.Initialize();
        }

        public List<Building> GetUnlockedBuildings()
        {
            List<Building> unlockedBuildings = new();
            foreach (Building b in _buildings)
                if (b.IsUnlocked())
                    unlockedBuildings.Add(b);
            return unlockedBuildings;
        }

        public Sprite VaseIcon;

        [Header("Pickups")] public QuestablePickup[] QuestablePickups;

        public QuestablePickup GetRandomQuestablePickup()
        {
            return QuestablePickups[Random.Range(0, QuestablePickups.Length)];
        }


        [Header("Shaders")] public Shader LitShader;
        public Shader ParticlesUnlitShader;
        public Shader DissolveShader;
        public Shader GrayScaleShader;
        public Shader SepiaToneShader;

        public List<Shader> KeepShadersMaterials = new();


        [SerializeField] ColorVariable[] _colors;

        public ColorVariable GetColorByName(string n)
        {
            return _colors.FirstOrDefault(c => c.name == n);
        }

        [FormerlySerializedAs("CoinSprites")] [SerializeField]
        Sprite[] _coinSprites;

        public Sprite[] LevelUpAnimationSprites;
        public Sprite[] TroopsElementAnimationSprites;

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


    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic
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
        ExpBonus
    }

    public enum ElementName
    {
        Fire,
        Water,
        Wind,
        Earth,
        Lightning,
        Metal,
        Wood,
        Ice,
        None
    }

    [System.Serializable]
    public struct StatBasics
    {
        public StatType StatType;
        public Sprite Sprite;
        public string Description;
    }

    [System.Serializable]
    public struct QuestablePickup
    {
        public Pickup Pickup;
        public Vector2Int AmountRange;
    }
}