using UnityEngine;
using System.Threading.Tasks;

public class CameraManager : MonoBehaviour
{
    BoardManager _boardManager;

    Camera _cam;

    Transform _followTarget;
    [SerializeField] float _moveSpeed;
    Vector3 _targetPos;

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

        _cam = GetComponent<Camera>();
        _boardManager = BattleManager.instance.GetComponent<BoardManager>();
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void Update()
    {
        if (_followTarget == null)
            return;

        // follow the target
        _targetPos = new Vector3(_followTarget.position.x, _followTarget.position.y, transform.position.z);
        Vector3 velocity = (_targetPos - transform.position) * _moveSpeed;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref velocity, 1.0f, Time.deltaTime);
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
        _cam.orthographicSize = 12;
        transform.position = new Vector3(_boardManager.MapSize.x / 2, _boardManager.MapSize.y / 2, -2); // TODO:
    }

    async void HandleDeployment()
    {
        _followTarget = MovePointController.instance.transform;
        await LerpOrthographicSize(7, 1);
    }

    void HandlePlayerTurn()
    {
        _followTarget = MovePointController.instance.transform;
    }

    public void SetTarget(Transform t)
    {
        _followTarget = t;
    }

    async Task LerpOrthographicSize(float newSize, float time)
    {
        float oldSize = _cam.orthographicSize;
        float elapsed = 0;
        while (elapsed <= time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            _cam.orthographicSize = Mathf.Lerp(oldSize, newSize, t);
            await Task.Yield();
        }
    }

}

