using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Lis.Units.Enemy;

#if UNITY_EDITOR


namespace Editor
{
    public class EditorEnemyStatSetter : MonoBehaviour
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

                // get the enemy stats
                int enemyMaxHealth = Convert.ToInt32(dict["Health"]);
                int enemyArmor = Convert.ToInt32(dict["Armor"]);
                float enemySpeed = float.Parse(dict["Speed"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                int enemyPower = Convert.ToInt32(dict["Power"]);
                // get the enemy object
                string enemyPath = "Assets/_Scripts/Units/Enemy/_ScriptableObjects/" + enemyName + ".asset";
                Enemy enemy = AssetDatabase.LoadAssetAtPath<Enemy>(enemyPath);
                // set the enemy stats
                enemy.EnemyMaxHealth = enemyMaxHealth;
                enemy.EnemyArmor = enemyArmor;
                enemy.EnemySpeed = enemySpeed;
                enemy.EnemyPower = enemyPower;
                // save the enemy object
                EditorUtility.SetDirty(enemy);
                AssetDatabase.SaveAssets();
            }
        }
    }
}

#endif