using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] JourneyNodeReward _rewardScriptableObject;
    JourneyNodeReward _reward;

    public static BattleManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        #endregion

        _reward = Instantiate(_rewardScriptableObject);
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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
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
