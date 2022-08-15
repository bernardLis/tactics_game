using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Threading.Tasks;

public class JourneyCameraController : MonoBehaviour
{
    Camera _cam;

    JourneyMapManager _journeyMapManager;
    PlayerInput _playerInput;

    bool _allowInput;

    void Awake()
    {
        _cam = GetComponent<Camera>();

        _journeyMapManager = JourneyMapManager.Instance;
        _journeyMapManager.OnInitialJourneySetup += OnInitialJourneySetup;
        _journeyMapManager.OnNodeSelection += OnNodeSelection;
    }

    void Start()
    {
        _playerInput = GameManager.Instance.GetComponent<PlayerInput>();
        UnsubscribeInputActions();
        SubscribeInputActions();

        // showing the whole journey
        Vector3 endPos = _journeyMapManager.EndNode.transform.position;
        transform.position = new Vector3(endPos.x, endPos.y * 0.5f, -10);
        _cam.orthographicSize = 270;
    }

    void OnInitialJourneySetup()
    {
        MoveCameraToStart();
    }

    void OnNodeSelection()
    {
        UnsubscribeInputActions();
    }

    void Update()
    {
        if (!_allowInput)
            return;

        Vector2 vec = Mouse.current.scroll.ReadValue();
        if (vec.y == 0)
            return;

        Zoom(vec.y);
    }

    /* INPUT */
    void UnsubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed -= Move;
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed += Move;
    }

    // TODO: this should be better
    void Move(InputAction.CallbackContext ctx)
    {
        if (!_allowInput)
            return;

        Vector3 change = Vector3.one * ctx.ReadValue<Vector2>() * 30f;
        Vector3 endPos = transform.position + change;
        transform.DOMove(endPos, 0.5f);
    }


    void Zoom(float value)
    {
        if (value > 0)
            _cam.DOOrthoSize(_cam.orthographicSize - 20, 1f);
        else
            _cam.DOOrthoSize(_cam.orthographicSize + 20, 1f);
    }

    async void MoveCameraToStart()
    {
        await Task.Delay(1000);
        // zooming in to the end node
        float duration = 3f;
        Vector3 endPos = _journeyMapManager.EndNode.transform.position;
        transform.DOMoveY(endPos.y, duration);
        _cam.DOOrthoSize(60, duration);
        await Task.Delay(Mathf.RoundToInt(duration * 1000));
        Vector3 startPos = _journeyMapManager.StartNode.transform.position;
        transform.DOMoveY(startPos.y, duration).OnComplete(() => _allowInput = true);
    }
}
