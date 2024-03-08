using System.Collections;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class GameStatsElement : VisualElement
    {
        const string _ussClassName = "game-stats__";
        const string _ussMinionsKilledIcon = _ussClassName + "minions-killed-icon";
        const string _ussCreaturesKilledIcon = _ussClassName + "creatures-killed-icon";
        const string _ussTilesUnlockedIcon = _ussClassName + "tiles-unlocked-icon";

        Stats _stats;

        ScrollView _scrollView;


        public GameStatsElement(Stats stats)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.GameStatsStyles);
            if (ss != null) styleSheets.Add(ss);

            _stats = stats;

            _scrollView = new();
            Add(_scrollView);

            AddStat("Minions Killed", _stats.MinionsKilled, _ussMinionsKilledIcon);
        }

        void AddStat(string text, int value, string iconClass)
        {
            VisualElement container = new();
            container.AddToClassList(_ussClassName + "stat-container");

            Label label = new();
            label.AddToClassList(_ussClassName + "stat-label");
            label.text = text;
            container.Add(label);

            Label valueLabel = new();
            valueLabel.AddToClassList(_ussClassName + "stat-value");
            valueLabel.text = value.ToString();
            container.Add(valueLabel);

            VisualElement icon = new();
            icon.AddToClassList(iconClass);
            container.Add(icon);

            _scrollView.Add(container);
        }
    }
}

/*
public int MinionsKilled;
public int CreaturesKilled;

public int TilesUnlocked;

public int VasesBroken;
public int CoinsCollected;
public int HammersCollected;
public int HorseshoesCollected;
public int BagsCollected;
public int SkullsCollected;
public int FriendBallsCollected;

public int CommonExpOrbsCollected;
public int UncommonExpOrbsCollected;
public int RareExpOrbsCollected;
public int EpicExpOrbsCollected;

public int FriendBallsThrown;
public List<MyObjectData> CreaturesCaptured = new();

public List<MyObjectData> TabletsCollected = new();
public List<MyObjectData> AdvancedTabletsCollected = new();

public List<MyObjectData> AbilitiesUnlocked = new();

public List<MyObjectData> BossesKilled = new();
*/