using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class AddressableManager : MonoBehaviour
    {
        [FormerlySerializedAs("StyleSheetReferences")] [SerializeField]
        List<AssetReference> _styleSheetReferences = new();

        readonly List<StyleSheet> _styleSheets = new();

        void Start()
        {
            // https://www.youtube.com/watch?v=0USXRC9f4Iw
            Addressables.InitializeAsync().Completed += AddressableManager_Completed;
        }

        void AddressableManager_Completed(
            AsyncOperationHandle<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator> obj)
        {
            foreach (AssetReference reference in _styleSheetReferences)
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
        CommonStyles,
        TooltipElementStyles,
        ConfirmPopupStyles,
        MenuStyles,
        SettingsScreenStyles,
        FinishedBattleStyles,
        TimerElementStyles,
        StarRankElementStyles,
        GoldElementStyles,
        ResourceBarStyles,
        StatElementStyles,
        NatureElementStyles,
        AbilityElementStyles,
        AbilityIconStyles,
        AbilityTooltipElementStyles,

        EntityInfoElementStyles,
        RewardScreenStyles,
        RewardElementStyles,

        UnitIconStyles,
        UnitCardStyles,
        UnitScreenStyles,

        CreatureAbilityStyles,
        CreatureAbilityTooltipStyles,

        UpgradeStyles,
        UpgradeScreenStyles,

        TurretIconStyles,
        StatsBattleElementStyles,
        TextPrintingStyles,
        HeroElementStyles,
        TooltipCardStyles,
        BuildingCardStyles,
        TabletElementStyles,
        TabletTooltipElementStyles,
        TabletAdvancedScreenStyles,

        HeroSelectorInfoStyles,
        StatsScreenStyles,

        InvestmentElementStyles,
    }
}