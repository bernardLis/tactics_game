using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class FaceDirectionUI : MonoBehaviour
{
    // global
    Camera cam;
    BattleCharacterController battleCharacterController;

    // local
    PlayerCharSelection playerCharSelection;

    // UI Elements
    UIDocument UIDocument;
    VisualElement container;
    Button faceUpButton;
    Button faceLeftButton;
    Button faceRightButton;
    Button faceDownButton;

    bool isUIShown;
    bool directionPicked;
    bool breakTask;
    Vector2 pickedDirection;

    // to place the UI in the right spot
    public float offsetY = 1.75f;
    public float offsetX = -1.42f;

    void Start()
    {
        // global
        cam = Camera.main;
        battleCharacterController = BattleCharacterController.instance;

        // local
        playerCharSelection = GetComponent<PlayerCharSelection>();

        // getting ui elements
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        container = root.Q<VisualElement>("faceDirectionContainer");

        faceUpButton = root.Q<Button>("faceUp");
        faceLeftButton = root.Q<Button>("faceLeft");
        faceRightButton = root.Q<Button>("faceRight");
        faceDownButton = root.Q<Button>("faceDown");

        faceUpButton.clickable.clicked += FaceUpButtonClicked;
        faceLeftButton.clickable.clicked += FaceLeftButtonClicked;
        faceRightButton.clickable.clicked += FaceRightButtonClicked;
        faceDownButton.clickable.clicked += FaceDownButtonClicked;
    }

    void Update()
    {
        if (isUIShown)
           ShowUI();

    }

    public async Task<Vector2> PickDirection()
    {
        MovePointController.instance.Move(transform.position);

        container.style.display = DisplayStyle.Flex;
        isUIShown = true;

        directionPicked = false;
        breakTask = false;

        // disable selection arrow
        playerCharSelection.ToggleSelectionArrow(false);

        battleCharacterController.UpdateCharacterState(CharacterState.SelectingFaceDir);

        while (!directionPicked)
        {
            if (breakTask)
                return Vector2.zero;

            await Task.Yield();
        }

        HideUI();

        return pickedDirection;
    }

    void ShowUI()
    {

        // TODO: place it on the character
        Vector3 middleOfTheTile = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(container.panel, middleOfTheTile, cam);

        container.transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public void HideUI()
    {
        isUIShown = false;
        breakTask = true;
        container.style.display = DisplayStyle.None;
    }

    void FaceUpButtonClicked()
    {
        directionPicked = true;
        pickedDirection = Vector2.up;
    }
    void FaceLeftButtonClicked()
    {
        directionPicked = true;
        pickedDirection = Vector2.left;
    }
    void FaceRightButtonClicked()
    {
        directionPicked = true;
        pickedDirection = Vector2.right;
    }
    void FaceDownButtonClicked()
    {
        directionPicked = true;
        pickedDirection = Vector2.down;
    }


    public void SimulateUpButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = faceUpButton })
            faceUpButton.SendEvent(e);
    }

    public void SimulateLeftButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = faceLeftButton })
            faceLeftButton.SendEvent(e);
    }

    public void SimulateRightButtonClicked()
    {
        // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
        using (var e = new NavigationSubmitEvent() { target = faceRightButton })
            faceRightButton.SendEvent(e);
    }

    public void SimulateDownButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = faceDownButton })
            faceDownButton.SendEvent(e);
    }


}
