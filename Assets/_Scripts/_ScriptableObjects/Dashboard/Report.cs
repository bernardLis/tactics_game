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

    public Report(string text)
    {
        ReportType = ReportType.Text;
        Text = text;
    }

    public void CreateFromData(ReportData data)
    {
        ReportType = (ReportType)System.Enum.Parse(typeof(ReportData), data.ReportType);

        // TODO: should game manager hold quest info and this is only id ?
        if (ReportType == ReportType.NewQuest || ReportType == ReportType.FinishedQuest)
        {
            Quest = ScriptableObject.CreateInstance<Quest>();
            Quest.CreateFromData(data.Quest);
            return;
        }

        if (ReportType == ReportType.Recruit)
        {
            Recruit = ScriptableObject.CreateInstance<Character>();
            Recruit.CreateFromData(data.Recruit);
            return;
        }

        if (ReportType == ReportType.Text)
            Text = data.Text;
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
}
