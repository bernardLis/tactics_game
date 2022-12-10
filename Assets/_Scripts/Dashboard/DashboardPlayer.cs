using UnityEngine;
using UnityEngine.InputSystem;

public class DashboardPlayer : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    Rigidbody2D rb;
    Vector2 moveInput;

    public float moveSpeed = 5f;

    public bool IsInputAllowed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();

        rb = GetComponent<Rigidbody2D>();
        IsInputAllowed = true;
    }

    void OnEnable()
    {
        // inputs
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Dashboard");

        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed += Move;
        _playerInput.actions["ArrowMovement"].canceled += MoveCanceled;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed -= Move;
        _playerInput.actions["ArrowMovement"].canceled -= MoveCanceled;
    }

    void Move(InputAction.CallbackContext ctx) { moveInput = ctx.ReadValue<Vector2>(); }

    void MoveCanceled(InputAction.CallbackContext ctx) { moveInput = ctx.ReadValue<Vector2>(); }

    void FixedUpdate()
    {
        if (!IsInputAllowed)
            return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetInputAllowed(bool isAllowed) { IsInputAllowed = isAllowed; }
}
