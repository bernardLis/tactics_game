using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class DamageUI : MonoBehaviour
{
    Camera cam;

    UIDocument UIDocument;
    float offsetY = 0.5f;

    VisualElement healthChangeDisplayContainer;
    Label healthChangeDisplayLabel;

    Queue<IEnumerator> coroutineQueue = new();

    void Awake()
    {
        // TODO: Supposedly, this is an expensive call
        cam = Helpers.Camera;

        UIDocument = GetComponent<UIDocument>();
        // getting ui elements
        var root = UIDocument.rootVisualElement;
        healthChangeDisplayContainer = root.Q<VisualElement>("healthChangeDisplayContainer");
        healthChangeDisplayLabel = root.Q<Label>("healthChangeDisplayLabel");
    }
    void Start()
    {
        StartCoroutine(CoroutineCoordinator());
    }

    public void DisplayOnCharacter(string _txt, int _fontSize, Color col)
    {
        coroutineQueue.Enqueue(DisplayOnCharacterCoroutine(_txt, _fontSize, col));
    }

    IEnumerator DisplayOnCharacterCoroutine(string _txt, int _fontSize, Color _col)
    {
        healthChangeDisplayContainer.style.display = DisplayStyle.Flex;

        healthChangeDisplayLabel.text = _txt;
        healthChangeDisplayLabel.style.fontSize = _fontSize;
        healthChangeDisplayLabel.style.color = _col;

        // set position of the element 
        Vector3 middleOfTheTile = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(healthChangeDisplayContainer.panel, middleOfTheTile, cam);

        float x = newPosition.x; //  - healthChangeDisplayContainer.resolvedStyle.width / 2; <- 0 on the first frame
        float y = newPosition.y; // - healthChangeDisplayContainer.layout.height; <- height is 0, coz it does not calculate in the first frame after element creation

        healthChangeDisplayContainer.transform.position = new Vector3(x, y, transform.position.z);

        // move up
        float waitTime = 0;
        float fadeTime = 1f;
        float i = 0.1f;
        while (waitTime < 1f)
        {
            middleOfTheTile = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);
            newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(healthChangeDisplayContainer.panel, middleOfTheTile, cam);

            // TODO: i am calculating x so many times is because size won't be computed on the first frame after creating an element
            // https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
            x = newPosition.x - healthChangeDisplayContainer.resolvedStyle.width / 2;
            y = newPosition.y - i;
            i += 0.1f;

            healthChangeDisplayContainer.transform.position = new Vector3(x, y, transform.position.z);

            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }

        healthChangeDisplayContainer.style.display = DisplayStyle.None;
    }

    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }



}
