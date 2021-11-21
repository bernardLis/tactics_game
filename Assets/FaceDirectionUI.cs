using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class FaceDirectionUI : MonoBehaviour
{
    // global
    Camera cam;

    // UI Elements
    UIDocument UIDocument;
    VisualElement container;
    Button faceUpButton;
    Button faceLeftButton;
    Button faceRightButton;
    Button faceDownButton;

    bool directionPicked;
    Vector2 pickedDirection;

    // to place the UI in the right spot
    float offsetY = 1.75f;
    float offsetX = -0.75f;


    void Start()
    {
        // global
        cam = Camera.main;

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

    public async Task<Vector2> PickDirection()
    {
        ShowUI();
        directionPicked = false;
        BattleInputController.instance.SetInputAllowed(false);
        while (!directionPicked)
        {
            await Task.Yield();
        }

        HideUI();
        BattleInputController.instance.SetInputAllowed(true);

        return pickedDirection;
    }

    void ShowUI()
    {
        container.style.display = DisplayStyle.Flex;

        // TODO: place it on the character
        Vector3 middleOfTheTile = new Vector3(transform.position.x+ offsetX, transform.position.y + offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(container.panel, middleOfTheTile, cam);

        container.transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public void HideUI()
    {
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


}
