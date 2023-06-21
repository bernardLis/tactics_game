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
    [SerializeField] AssetReference[] StyleSheetReferences;
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
    CutsceneStyles,
    ElementChoiceStyles,
    TimerElementStyles,
    BattleResultStyles,
    BarMiniGameStyles,
    StarRankElementStyles,
    GoldElementStyles, SpiceElementStyles,
    HeroPortraitStyles, ResourceBarStyles, StatElementStyles, ElementalElementStyles,
    AbilityButtonStyles, AbilityIconStyles, AbilityTooltipElementStyles,
    ItemSlotStyles, AbilitySlotStyles,
    HeroCardStatsStyles,
    HeroCardMiniStyles,
    HeroCardExpStyles,

    CastleElementStyles,
    BuildingElementStyles, ProductionBuildingElementStyles, BuildingSpriteElementStyles,
    ArmyGroupElementStyles, ArmyGroupTooltipStyles, ArmySlotElementStyles, ArmyBuyableElementStyles, ArmyEvolutionElementStyles,
    HeroCardFullStyles, HeroArmyElementStyles,
    ControlButtonStyles, CastleControlButtonStyles, HeroControlButtonStyles,

    BattleEntityElementStyles, BattleEntityInfoStyles,
    BattleResultArmyStyles, BattleResultRewardStyles, BattleResultHeroStyles,
    RewardCardStyles, BattleCardStyles,

    CreatureIconStyles, CreatureElementStyles,

    CreatureAbilityStyles, CreatureAbilityTooltipStyles,

}
