using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class BattleCameraManager : Singleton<BattleCameraManager>
{
    BoardManager _boardManager;
    MovePointController _movePointController;

    protected Camera _cam;

    Transform _followTarget;
    Vector3 velocity = Vector3.zero;

    Vector3 _targetPos;

    protected override void Awake()
    {
        base.Awake();
        _cam = GetComponent<Camera>();
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        _boardManager = BoardManager.Instance;
    }

    protected virtual void Start()
    {
        _movePointController = MovePointController.Instance;
    }

    void Update()
    {
        if (_followTarget == null)
            return;

        // follow the target
        _targetPos = new Vector3(_followTarget.position.x, _followTarget.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref velocity, 0.25f);
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.MapBuilding)
            HandleMapBuilding();
        if (state == BattleState.Deployment)
            HandleDeployment();
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    void HandleMapBuilding()
    {
        _cam.orthographicSize = 12;
        transform.position = new Vector3(_boardManager.MapSize.x / 2, _boardManager.MapSize.y / 2, -2);
    }

    async void HandleDeployment()
    {
        _followTarget = _movePointController.transform;
        await LerpOrthographicSize(6, 1);
    }

    void HandlePlayerTurn()
    {
        _followTarget = _movePointController.transform;
    }

    public void SetTarget(Transform t)
    {
        _followTarget = t;
    }

    //TODO: DOOrthoSize(float to, float duration)
    public async Task LerpOrthographicSize(float newSize, float time)
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
    public void Shake()
    {
        transform.DOShakePosition(0.5f, 0.5f);
    }

}

