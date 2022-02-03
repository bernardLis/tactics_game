using UnityEngine;
using System.Threading.Tasks;

public class CameraManager : MonoBehaviour
{

    public Transform followTarget;
    Vector3 targetPos;
    public float moveSpeed;

    BoardManager boardManager;

    public static CameraManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Camera Follow found");
            return;
        }
        instance = this;
        #endregion

        boardManager = GameManager.instance.GetComponent<BoardManager>();
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void Update()
    {
        if (followTarget == null)
            return;

        // follow the target
        targetPos = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
        Vector3 velocity = (targetPos - transform.position) * moveSpeed;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState _state)
    {
        if (_state == BattleState.MapBuilding)
            HandleMapBuilding();
        if (_state == BattleState.Deployment)
            HandleDeployment();
        if (_state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    void HandleMapBuilding()
    {
        transform.position = new Vector3(boardManager.mapSize.x / 2, boardManager.mapSize.y / 2, -2); // TODO:
    }

    async void  HandleDeployment()
    {
        followTarget = MovePointController.instance.transform;
        await ResizeRoutine(12, 7, 1);
    }

    void HandlePlayerTurn()
    {
        followTarget = MovePointController.instance.transform;
    }

    async Task ResizeRoutine(float oldSize, float newSize, float time)
    {
        float elapsed = 0;
        while (elapsed <= time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            Camera.main.orthographicSize = Mathf.Lerp(oldSize, newSize, t);
            await Task.Yield();
        }
    }

}

