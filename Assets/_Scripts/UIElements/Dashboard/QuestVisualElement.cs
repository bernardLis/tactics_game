using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestVisualElement : VisualElement
{
    Quest _quest;

    VisualElement _additionalInfo;
    bool _isAdditionalInfoShown;

    // TODO: would be cool if it was showing only icon and title and on click you would get more information
    // and there would be a button to commence / assign people
    public QuestVisualElement(Quest quest)
    {
        _quest = quest;
        AddToClassList("questElement");
        AddToClassList("textPrimary");

        // basics
        VisualElement basicInfoContainer = new();
        basicInfoContainer.AddToClassList("questBasicInfoContainer");
        Add(basicInfoContainer);

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(quest.Icon);
        icon.style.width = 50;
        icon.style.height = 50;
        basicInfoContainer.Add(icon);

        Label title = new(quest.Title);
        title.AddToClassList("textPrimary");
        basicInfoContainer.Add(title);

        CreateAdditionalInfo();

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (_isAdditionalInfoShown)
        {
            _isAdditionalInfoShown = false;
            Remove(_additionalInfo);
            return;
        }

        Add(_additionalInfo);
        _isAdditionalInfoShown = true;
    }


    void CreateAdditionalInfo()
    {
        _additionalInfo = new();
        _additionalInfo.AddToClassList("questBasicInfoContainer");

        // map info
        VisualElement mapInfoContainer = new();
        _additionalInfo.Add(mapInfoContainer);

        Label biome = new($"Biome: {_quest.Biome.name}");
        mapInfoContainer.Add(biome);

        Label variant = new($"Variant: {_quest.MapVariant.name}");
        mapInfoContainer.Add(variant);

        Label mapSize = new($"Map size: {_quest.MapSize.x} x {_quest.MapSize.y}");
        mapInfoContainer.Add(mapSize);

        VisualElement enemyIconContainer = new VisualElement();
        enemyIconContainer.style.flexDirection = FlexDirection.Row;
        mapInfoContainer.Add(enemyIconContainer);

        foreach (var e in _quest.Enemies)
        {
            Label l = new Label();
            l.style.backgroundImage = e.BrainIcon.texture;
            l.style.width = 32;
            l.style.height = 32;
            enemyIconContainer.Add(l);
        }

        // reward
        VisualElement rewardContainer = new();
        rewardContainer.style.flexDirection = FlexDirection.Row;
        rewardContainer.style.alignContent = Align.Center;
        _additionalInfo.Add(rewardContainer);

        if (_quest.Reward.Gold != 0)
            rewardContainer.Add(new GoldElement(_quest.Reward.Gold));
        if (_quest.Reward.Item != null)
            rewardContainer.Add(new ItemSlotVisual(new ItemVisual(_quest.Reward.Item)));

    }

}
