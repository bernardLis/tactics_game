using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] List<AssetReference> StyleSheetReferences = new();
    List<StyleSheet> _styleSheets = new();

    void Start()
    {
        // https://www.youtube.com/watch?v=0USXRC9f4Iw
        Addressables.InitializeAsync().Completed += AddressableManager_Completed;
    }

    void AddressableManager_Completed(AsyncOperationHandle<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator> obj)
    {
        foreach (AssetReference reference in StyleSheetReferences)
        {
            if (reference == null)
            {
                Debug.LogWarning($"Missing addressable reference.");
                continue;
            }
            
            reference.LoadAssetAsync<StyleSheet>().Completed += (sheet) =>
            {
                _styleSheets.Add((StyleSheet)sheet.Result);
            };
        }
    }

    public StyleSheet GetStyleSheetByName(StyleSheetType name)
    {
        return _styleSheets.FirstOrDefault(x => x.name == name.ToString());
    }
}

public enum StyleSheetType
{
    CommonStyles, TooltipElementStyles, ConfirmPopupStyles,
    MenuStyles, SettingsMenuStyles,
    FinishedBattleScreenStyles,
    TimerElementStyles,
    StarRankElementStyles,
    GoldElementStyles,
    ResourceBarStyles, StatElementStyles, ElementalElementStyles,
    AbilityElementStyles, AbilityIconStyles, AbilityTooltipElementStyles,

    EntityInfoElementStyles,
    LevelUpScreenStyles,
    RewardElementStyles,

    EntityIconStyles, EntityCardStyles, EntityScreenStyles,

    CreatureAbilityStyles, CreatureAbilityTooltipStyles,

    UpgradeStyles, UpgradeScreenStyles,

    TurretIconStyles,
    StatsBattleElementStyles,
    TextPrintingStyles,
    HeroElementStyles,
    TooltipCardStyles, BuildingCardStyles,



}
