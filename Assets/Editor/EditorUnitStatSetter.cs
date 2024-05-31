using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Lis.Units.Enemy;
using Lis.Units.Pawn;

#if UNITY_EDITOR


namespace Editor
{
    public class EditorUnitStatSetter : MonoBehaviour
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
                if (pawnName == "Peasant") continue;

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
    }
}

#endif