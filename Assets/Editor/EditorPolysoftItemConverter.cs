using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Lis.Units.Hero.Items;

#if UNITY_EDITOR

namespace Editor
{
    public class EditorPolysoftItemConverter : MonoBehaviour
    {
        [MenuItem("Utilities/Polysoft Female Items Converter")]
        static void ConvertFemaleItems()
        {
            ConvertFemaleArmor();
            ConvertFemaleHair();
            ConvertFemaleUnderwear();
        }


        [MenuItem("Utilities/Polysoft Female OutfitConverter")]
        static void ConvertFemaleOutfit()
        {
            string itemPath = "Assets/_Scripts/Units/Hero/Items/_ScriptableObjects/Female/Outfit";
            string path =
                "Assets/Plugins/Models/Modular RPG Character Polyart/Modular Character Female/Items/Outfit";
            string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                polysoft_Item polysoftItem = AssetDatabase.LoadAssetAtPath<polysoft_Item>(file);

                // create a new item
                Item newItem = ScriptableObject.CreateInstance<Item>();

                newItem.ItemType = (ItemType)(int)polysoftItem.itemType;
                newItem.ArmorType = (ArmorType)(int)polysoftItem.armorType;
                newItem.DisableHair = polysoftItem.disableHair;
                newItem.DisableBeard = polysoftItem.disableBeard;
                newItem.DisableMustache = polysoftItem.disableMustache;

                newItem.ItemMeshRenderer = polysoftItem.ItemMeshRenderer;
                AssetDatabase.CreateAsset(newItem, itemPath + "/" + ParseName(polysoftItem.name) + ".asset");
                EditorUtility.SetDirty(newItem);
                AssetDatabase.SaveAssets();
            }

        }

        static void ConvertFemaleArmor()
        {
            string itemPath = "Assets/_Scripts/Units/Hero/Items/_ScriptableObjects/Female/Armor";

            string path =
                "Assets/Plugins/Models/Modular RPG Character Polyart/Modular Character Female/Items/Armor";
            string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                polysoft_Item polysoftItem = AssetDatabase.LoadAssetAtPath<polysoft_Item>(file);

                // create a new item
                Item newItem = ScriptableObject.CreateInstance<Item>();

                newItem.ItemType = (ItemType)(int)polysoftItem.itemType;
                newItem.ArmorType = (ArmorType)(int)polysoftItem.armorType;
                newItem.DisableHair = polysoftItem.disableHair;
                newItem.DisableBeard = polysoftItem.disableBeard;
                newItem.DisableMustache = polysoftItem.disableMustache;

                newItem.ItemMeshRenderer = polysoftItem.ItemMeshRenderer;
                AssetDatabase.CreateAsset(newItem, itemPath + "/" + ParseName(polysoftItem.name) + ".asset");
                EditorUtility.SetDirty(newItem);
                AssetDatabase.SaveAssets();
            }
        }

        static void ConvertFemaleHair()
        {
            string itemPath = "Assets/_Scripts/Units/Hero/Items/_ScriptableObjects/Female/Hair";

            string path =
                "Assets/Plugins/Models/Modular RPG Character Polyart/Modular Character Female/Items/Hair";
            string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                polysoft_Hair polysoftItem = AssetDatabase.LoadAssetAtPath<polysoft_Hair>(file);

                // create a new item
                Item newItem = ScriptableObject.CreateInstance<Item>();

                newItem.ItemType = ParseItemTypeForHair(polysoftItem);
                newItem.ItemMeshRenderer = polysoftItem.ItemMeshRenderer;
                AssetDatabase.CreateAsset(newItem, itemPath + "/" + ParseName(polysoftItem.name) + ".asset");
                EditorUtility.SetDirty(newItem);
                AssetDatabase.SaveAssets();
            }
        }

        static void ConvertFemaleUnderwear()
        {
            string itemPath = "Assets/_Scripts/Units/Hero/Items/_ScriptableObjects/Female/Underwear";
            string path =
                "Assets/Plugins/Models/Modular RPG Character Polyart/Modular Character Female/Items/Underwear";
            string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                polysoft_Underwear polysoftItem = AssetDatabase.LoadAssetAtPath<polysoft_Underwear>(file);

                // create a new item
                Item newItem = ScriptableObject.CreateInstance<Item>();

                newItem.ItemType = ParseItemTypeForUnderwear(polysoftItem);
                newItem.ItemMeshRenderer = polysoftItem.ItemMeshRenderer;
                AssetDatabase.CreateAsset(newItem, itemPath + "/" + ParseName(polysoftItem.name) + ".asset");
                EditorUtility.SetDirty(newItem);
                AssetDatabase.SaveAssets();
            }
        }

        static ItemType ParseItemTypeForUnderwear(polysoft_Underwear polysoftItem)
        {
            if (polysoftItem.itemType == polysoft_Underwear.underwearType.pants)
                return ItemType.Underwear;
            if (polysoftItem.itemType == polysoft_Underwear.underwearType.brassiere)
                return ItemType.Brassiere;

            throw new NotImplementedException();
        }


        static ItemType ParseItemTypeForHair(polysoft_Hair polysoftItem)
        {
            if (polysoftItem.itemType == polysoft_Hair.hairType.hair)
                return ItemType.Hair;
            if (polysoftItem.itemType == polysoft_Hair.hairType.beard)
                return ItemType.Beard;
            if (polysoftItem.itemType == polysoft_Hair.hairType.mustache)
                return ItemType.Mustache;

            throw new NotImplementedException();
        }


        static string ParseName(string polysoftItemName)
        {
            string returnName = "";
            string[] nameSplit = polysoftItemName.Split("_");
            foreach (string s in nameSplit)
            {
                returnName += Capitalize(s);
                returnName += " ";
            }

            return returnName.Trim();
        }

        static string Capitalize(string s)
        {
            return s.First().ToString().ToUpper() + s.Substring(1);
        }
    }
}
#endif