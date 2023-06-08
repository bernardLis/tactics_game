using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene")]
public class Cutscene : BaseScriptableObject
{
    public CutsceneLine[] Lines;

    public void Initialize()
    {
        Debug.Log($"Initializing cutscene {name} with # lines {Lines.Length}");
        
        foreach (CutsceneLine l in Lines)
            l.Initialize();
    }
}