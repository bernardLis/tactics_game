using System;
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : Singleton<BattleManager>
{
    BattleInputController _battleInputController;

    public bool IsGamePaused { get; private set; }

    public event Action OnGamePaused;
    public event Action OnGameResumed;

    protected override void Awake() { base.Awake(); }

    void Start() { _battleInputController = BattleInputController.Instance; }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0;
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1;
        OnGameResumed?.Invoke();
    }

    public void SnapToGrid(Transform t)
    {
        float x = t.position.x;
        float y = t.position.y;
        //https://answers.unity.com/questions/714197/round-to-05-15-25-.html
        float outputX = Mathf.Sign(x) * (Mathf.Abs((int)x) + 0.5f);
        float outputY = Mathf.Sign(y) * (Mathf.Abs((int)y) + 0.5f);

        if (outputX != t.position.x || outputY != t.position.y)
            t.position = new Vector3(outputX, outputY, t.position.z);
    }
}
