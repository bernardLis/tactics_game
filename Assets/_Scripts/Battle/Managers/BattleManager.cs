using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    BattleInputController _battleInputController;

    [SerializeField] JourneyNodeReward _rewardScriptableObject;
    JourneyNodeReward _reward;

    protected override void Awake()
    {
        base.Awake();

        _reward = Instantiate(_rewardScriptableObject);
    }

    void Start()
    {
        _battleInputController = BattleInputController.Instance;
    }

    public void AddRewardObols(int n)
    {
        _reward.obols += n;
    }

    public JourneyNodeReward GetReward()
    {
        return _reward;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _battleInputController.SetInputAllowed(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        _battleInputController.SetInputAllowed(true);
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
