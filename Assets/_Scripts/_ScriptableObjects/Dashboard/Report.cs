using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Report : BaseScriptableObject
{
    public ReportType ReportType;
    public ReportPaper ReportPaper;

    public Quest Quest;
    public Character Recruit;
    public string Text;

    public bool IsSigned;
    public int DaySigned;
    public bool WasAccepted;

    public void Initialize(ReportType type, Quest quest = null, Character recruit = null, string text = null)
    {
        ReportType = type;
        ReportPaper = GameManager.Instance.GameDatabase.GetRandomReportPaper();

        Quest = quest;
        Recruit = recruit;
        Text = text;
    }

    public void Sign()
    {
        IsSigned = true;
        DaySigned = GameManager.Instance.Day;
    }

    public void CreateFromData(ReportData data)
    {
        ReportType = (ReportType)System.Enum.Parse(typeof(ReportType), data.ReportType);
        ReportPaper = GameManager.Instance.GameDatabase.GetReportPaperById(data.ReportPaperId);

        if (ReportType == ReportType.Quest || ReportType == ReportType.FinishedQuest || ReportType == ReportType.ExpiredQuest)
        {
            Quest = ScriptableObject.CreateInstance<Quest>();
            Quest.CreateFromData(data.Quest);
        }

        if (ReportType == ReportType.Recruit)
        {
            Recruit = ScriptableObject.CreateInstance<Character>();
            Recruit.CreateFromData(data.Recruit);
        }

        if (ReportType == ReportType.Text)
            Text = data.Text;

        IsSigned = data.IsSigned;
        DaySigned = data.DaySigned;
        WasAccepted = data.WasAccepted;
    }

    public ReportData SerializeSelf()
    {
        ReportData rd = new ReportData();

        rd.ReportType = ReportType.ToString();
        rd.ReportPaperId = ReportPaper.Id;
        if (Quest != null)
            rd.Quest = Quest.SerializeSelf();

        if (Recruit != null)
            rd.Recruit = Recruit.SerializeSelf();

        if (Text != null)
            rd.Text = Text;

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

    public QuestData Quest;

    public CharacterData Recruit;

    public string Text;

    public bool IsSigned;
    public int DaySigned;
    public bool WasAccepted;
}
