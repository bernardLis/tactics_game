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
        _playerInput.SwitchCurrentActionMap("Dashboard");

        rb = GetComponent<Rigidbody2D>();
        IsInputAllowed = true;
    }

    void FixedUpdate()
    {
        if (!IsInputAllowed)
            return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetInputAllowed(bool isAllowed) { IsInputAllowed = isAllowed; }

    // Assigned in editor... :(
    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
}
