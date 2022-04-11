using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class FaceDirectionUI : MonoBehaviour
{
    // global
    Camera _cam;
    BattleCharacterController _battleCharacterController;

    // local
    PlayerCharSelection _playerCharSelection;

    // UI Elements
    VisualElement _container;
    Button _faceUpButton;
    Button _faceLeftButton;
    Button _faceRightButton;
    Button _faceDownButton;

    bool _isUIShown;
    bool _isDirectionPicked;
    bool _breakTask;
    Vector2 _pickedDirection;

    // to place the UI in the right spot
    [SerializeField] float _offsetY = 1.75f;
    [SerializeField] float _offsetX = -1.42f;

    void Start()
    {
        // global
        _cam = Camera.main;
        _battleCharacterController = BattleCharacterController.Instance;

        // local
        _playerCharSelection = GetComponent<PlayerCharSelection>();

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _container = root.Q<VisualElement>("faceDirectionContainer");

        _faceUpButton = root.Q<Button>("faceUp");
        _faceLeftButton = root.Q<Button>("faceLeft");
        _faceRightButton = root.Q<Button>("faceRight");
        _faceDownButton = root.Q<Button>("faceDown");

        _faceUpButton.clickable.clicked += FaceUpButtonClicked;
        _faceLeftButton.clickable.clicked += FaceLeftButtonClicked;
        _faceRightButton.clickable.clicked += FaceRightButtonClicked;
        _faceDownButton.clickable.clicked += FaceDownButtonClicked;
    }

    void Update()
    {
        if (_isUIShown)
           ShowUI();
    }

    public async Task<Vector2> PickDirection()
    {
        MovePointController.Instance.Move(transform.position);

        _container.style.display = DisplayStyle.Flex;
        _isUIShown = true;

        _isDirectionPicked = false;
        _breakTask = false;

        // disable selection arrow
        _playerCharSelection.ToggleSelectionArrow(false);

        _battleCharacterController.UpdateCharacterState(CharacterState.SelectingFaceDir);

        while (!_isDirectionPicked)
        {
            if (_breakTask)
                return Vector2.zero;

            await Task.Yield();
        }

        HideUI();

        return _pickedDirection;
    }

    void ShowUI()
    {
        // TODO: place it on the character
        Vector3 middleOfTheTile = new Vector3(transform.position.x + _offsetX, transform.position.y + _offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(_container.panel, middleOfTheTile, _cam);

        _container.transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public void HideUI()
    {
        _isUIShown = false;
        _breakTask = true;
        _container.style.display = DisplayStyle.None;
    }

    void FaceUpButtonClicked()
    {
        _isDirectionPicked = true;
        _pickedDirection = Vector2.up;
    }

    void FaceLeftButtonClicked()
    {
        _isDirectionPicked = true;
        _pickedDirection = Vector2.left;
    }
    
    void FaceRightButtonClicked()
    {
        _isDirectionPicked = true;
        _pickedDirection = Vector2.right;
    }

    void FaceDownButtonClicked()
    {
        _isDirectionPicked = true;
        _pickedDirection = Vector2.down;
    }

    public void SimulateUpButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _faceUpButton })
            _faceUpButton.SendEvent(e);
    }

    public void SimulateLeftButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _faceLeftButton })
            _faceLeftButton.SendEvent(e);
    }

    public void SimulateRightButtonClicked()
    {
        // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
        using (var e = new NavigationSubmitEvent() { target = _faceRightButton })
            _faceRightButton.SendEvent(e);
    }

    public void SimulateDownButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _faceDownButton })
            _faceDownButton.SendEvent(e);
    }


}
