using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Report : BaseScriptableObject
{
    public ReportType ReportType;

    public Quest Quest;
    public Character Recruit;
    public string Text;

    public bool IsSigned;
    public int DaySigned;
    public bool WasAccepted;

    public void Sign()
    {
        IsSigned = true;
        DaySigned = GameManager.Instance.Day;
    }

    public void CreateFromData(ReportData data)
    {
        ReportType = (ReportType)System.Enum.Parse(typeof(ReportType), data.ReportType);

        if (ReportType == ReportType.NewQuest || ReportType == ReportType.FinishedQuest || ReportType == ReportType.ExpiredQuest)
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

    public QuestData Quest;

    public CharacterData Recruit;

    public string Text;

    public bool IsSigned;
    public int DaySigned;
    public bool WasAccepted;

}
