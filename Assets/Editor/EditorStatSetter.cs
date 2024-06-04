using System;
using System.Collections.Generic;
using Lis.Units;
using Lis.Units.Attack;
using UnityEngine;
using UnityEditor;
using Lis.Units.Enemy;
using Lis.Units.Pawn;
using Lis.Units.Peasant;

#if UNITY_EDITOR


namespace Editor
{
    public class EditorStatSetter : MonoBehaviour
    {
        [MenuItem("Utilities/Enemy Stat Setter")]
        static void SetEnemyStats()
        {
            // load the csv file from resources
            string path = "Assets/Resources/EnemyStats.csv";
            List<Dictionary<string, string>> data = CSVReader.Read(path);
            foreach (var dict in data)
            {
                // get the enemy name
                string enemyName = dict["Name"];
                if (enemyName.Length == 0)
                    continue;

                string enemyPath = "Assets/_Scripts/Units/Enemy/_ScriptableObjects/" + enemyName + ".asset";
                Enemy enemy = AssetDatabase.LoadAssetAtPath<Enemy>(enemyPath);
                // set the enemy stats
                enemy.Price = Convert.ToInt32(dict["Price"]);
                enemy.IsRanged = Convert.ToBoolean(dict["IsRanged"]);

                enemy.EnemyMaxHealth = Convert.ToInt32(dict["Health"]);
                enemy.EnemyArmor = Convert.ToInt32(dict["Armor"]);
                enemy.EnemySpeed = float.Parse(dict["Speed"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                enemy.EnemyPower = Convert.ToInt32(dict["Power"]);

                Debug.Log("Setting stats to: " + enemy.name);

                // save the enemy object
                EditorUtility.SetDirty(enemy);
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("Utilities/Pawn Stat Setter")]
        static void SetPawnStats()
        {
            string path = "Assets/Resources/PawnStats.csv";
            List<Dictionary<string, string>> data = CSVReader.Read(path);
            foreach (var dict in data)
            {
                // get the enemy name
                string pawnName = dict["Name"];
                string nature = dict["Nature"];

                if (pawnName.Length == 0) continue;
                if (pawnName == "Peasant") SetPeasantStats(dict);

                string pawnPath = $"Assets/_Scripts/Units/Pawn/_ScriptableObjects/{nature}/{pawnName}.asset";
                PawnUpgrade pawn = AssetDatabase.LoadAssetAtPath<PawnUpgrade>(pawnPath);

                Debug.Log("Setting stats to: " + pawn.name);
                pawn.Price = Convert.ToInt32(dict["Price"]);

                pawn.BaseHealth = Convert.ToInt32(dict["Health"]);
                pawn.BaseArmor = Convert.ToInt32(dict["Armor"]);
                pawn.BaseSpeed = float.Parse(dict["Speed"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                pawn.BasePower = Convert.ToInt32(dict["Power"]);

                // save the pawn object
                EditorUtility.SetDirty(pawn);
                AssetDatabase.SaveAssets();
            }
        }

        static void SetPeasantStats(Dictionary<string, string> dict)
        {
            Debug.Log("Setting stats to peasant");

            string armorPath = "Assets/_Scripts/Units/Peasant/_ScriptableObjects/Stats/Armor.asset";
            Stat armorStat = AssetDatabase.LoadAssetAtPath<Stat>(armorPath);
            armorStat.BaseValue = Convert.ToInt32(dict["Armor"]);
            EditorUtility.SetDirty(armorStat);
            AssetDatabase.SaveAssets();

            string health = "Assets/_Scripts/Units/Peasant/_ScriptableObjects/Stats/Max Health.asset";
            Stat healthStat = AssetDatabase.LoadAssetAtPath<Stat>(health);
            healthStat.BaseValue = Convert.ToInt32(dict["Health"]);
            EditorUtility.SetDirty(healthStat);
            AssetDatabase.SaveAssets();

            string power = "Assets/_Scripts/Units/Peasant/_ScriptableObjects/Stats/Power.asset";
            Stat powerStat = AssetDatabase.LoadAssetAtPath<Stat>(power);
            powerStat.BaseValue = Convert.ToInt32(dict["Power"]);
            EditorUtility.SetDirty(powerStat);
            AssetDatabase.SaveAssets();

            string speed = "Assets/_Scripts/Units/Peasant/_ScriptableObjects/Stats/Speed.asset";
            Stat speedStat = AssetDatabase.LoadAssetAtPath<Stat>(speed);
            speedStat.BaseValue = float.Parse(dict["Speed"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            EditorUtility.SetDirty(speedStat);
            AssetDatabase.SaveAssets();

            string peasant = "Assets/_Scripts/Units/Peasant/_ScriptableObjects/Peasant.asset";
            Peasant peasantSo = AssetDatabase.LoadAssetAtPath<Peasant>(peasant);
            peasantSo.Price = Convert.ToInt32(dict["Price"]);
            EditorUtility.SetDirty(peasantSo);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Utilities/Hero Ability Stat Setter")]
        static void SetHeroAbilityStats()
        {
            string path = "Assets/Resources/AbilityStats.csv";
            List<Dictionary<string, string>> data = CSVReader.Read(path);
            foreach (var dict in data)
            {
                string abilityName = dict["Name"];
                string nature = dict["Nature"];
                string level = dict["Level"];

                if (abilityName.Length == 0) continue;

                string assetPath =
                    $"Assets/_Scripts/Units/Hero/Ability/_ScriptableObjects/{nature}/{abilityName}/{level}.asset";
                AttackHeroAbility aha = AssetDatabase.LoadAssetAtPath<AttackHeroAbility>(assetPath);

                Debug.Log($"Setting stats to: {abilityName} - {level}");
                aha.Damage = Convert.ToInt32(dict["Damage"]);
                aha.Cooldown = float.Parse(dict["Cooldown"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                aha.IsArmorPiercing = Convert.ToBoolean(dict["Armor piercing"]);
                aha.Amount = Convert.ToInt32(dict["Amount"]);
                aha.Duration = float.Parse(dict["Duration"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                aha.Scale = float.Parse(dict["Scale"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                aha.Price = Convert.ToInt32(dict["Price"]);

                // save the pawn object
                EditorUtility.SetDirty(aha);
                AssetDatabase.SaveAssets();
            }
        }
    }
}

#endif