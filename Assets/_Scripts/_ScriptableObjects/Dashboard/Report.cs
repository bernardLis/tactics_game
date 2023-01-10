using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Report : BaseScriptableObject
{
    GameManager _gameManager;

    public ReportType ReportType;
    public ReportPaper ReportPaper;
    public Vector2 Position;

    public Quest Quest;
    public Recruit Recruit;
    public string Text;
    public string CampBuildingId;
    public Shop Shop;
    public Ability Ability;

    public bool IsSigned;
    public int DaySigned;
    public bool WasAccepted;

    public void Initialize(ReportType type, Quest quest = null, Recruit recruit = null, string text = null, string campBuildingId = null, Shop shop = null, Ability ability = null)
    {
        ReportType = type;
        ReportPaper = GameManager.Instance.GameDatabase.GetRandomReportPaper();

        Quest = quest;
        Recruit = recruit;
        Text = text;
        CampBuildingId = campBuildingId;
        Shop = shop;
        Ability = ability;
    }

    public void Sign()
    {
        IsSigned = true;
        DaySigned = GameManager.Instance.Day;
    }

    public void CreateFromData(ReportData data)
    {
        _gameManager = GameManager.Instance;

        ReportType = (ReportType)System.Enum.Parse(typeof(ReportType), data.ReportType);
        ReportPaper = GameManager.Instance.GameDatabase.GetReportPaperById(data.ReportPaperId);
        Position = data.Position;

        if (ReportType == ReportType.Quest)
        {
            Quest = ScriptableObject.CreateInstance<Quest>();
            Quest.LoadFromData(data.Quest);
        }
        if (ReportType == ReportType.Recruit)
        {
            Recruit = ScriptableObject.CreateInstance<Recruit>();
            Recruit.LoadFromData(data.Recruit);
        }

        if (ReportType == ReportType.Text)
            Text = data.Text;

        if (ReportType == ReportType.CampBuilding)
        {
            CampBuildingId = data.CampBuildingId;
        }
        if (ReportType == ReportType.Shop)
        {
            Shop = ScriptableObject.CreateInstance<Shop>();
            Shop.LoadFromData(data.ShopData);
        }
        if (ReportType == ReportType.Ability)
        {
            Ability = Instantiate(_gameManager.GameDatabase.GetAbilityById(data.AbilityData.TemplateId));
            Ability.name = data.AbilityData.Name;
        }

        IsSigned = data.IsSigned;
        DaySigned = data.DaySigned;
        WasAccepted = data.WasAccepted;
    }

    public ReportData SerializeSelf()
    {
        ReportData rd = new ReportData();

        rd.ReportType = ReportType.ToString();
        rd.ReportPaperId = ReportPaper.Id;
        rd.Position = Position;

        if (Quest != null)
            rd.Quest = Quest.SerializeSelf();

        if (Recruit != null)
            rd.Recruit = Recruit.SerializeSelf();

        if (Text != null)
            rd.Text = Text;

        if (CampBuildingId != null)
            rd.CampBuildingId = CampBuildingId;

        if (Shop != null)
            rd.ShopData = Shop.SerializeSelf();

        if (Ability != null)
            rd.AbilityData = Ability.SerializeSelf();

        rd.IsSigned = IsSigned;
        rd.DaySigned = DaySigned;
        rd.WasAccepted = WasAccepted;

        return rd;
    }
}

[Serializable]
public struct ReportData
{
    public string ReportType;
    public string ReportPaperId;
    public Vector2 Position;

    public QuestData Quest;
    public RecruitData Recruit;
    public string Text;
    public string CampBuildingId;
    public ShopData ShopData;
    public AbilityData AbilityData;

    public bool IsSigned;
    public int DaySigned;
    public bool WasAccepted;
}
