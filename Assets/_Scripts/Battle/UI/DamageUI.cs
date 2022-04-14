using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageUI : MonoBehaviour
{
    Camera _cam;

    VisualElement _healthChangeDisplayContainer;
    Label _healthChangeDisplayLabel;

    Queue<IEnumerator> _coroutineQueue = new();
    float _offsetY = 0.5f;

    void Start()
    {
        _cam = Helpers.Camera;

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;
        _healthChangeDisplayContainer = root.Q<VisualElement>("healthChangeDisplayContainer");
        _healthChangeDisplayLabel = root.Q<Label>("healthChangeDisplayLabel");

        StartCoroutine(CoroutineCoordinator());
    }

    // TODO: I could be displaying some effects on character - poison cloud or something 
    // and add it to the queue as well...

    public void DisplayOnCharacter(string txt, int fontSize, Color col)
    {
        _coroutineQueue.Enqueue(DisplayOnCharacterCoroutine(txt, fontSize, col));
    }

    IEnumerator DisplayOnCharacterCoroutine(string txt, int fontSize, Color col)
    {
        // TODO: use dotween instead
        _healthChangeDisplayContainer.style.display = DisplayStyle.Flex;

        _healthChangeDisplayLabel.text = txt;
        _healthChangeDisplayLabel.style.fontSize = fontSize;
        _healthChangeDisplayLabel.style.color = col;

        // set position of the element 
        Vector3 middleOfTheTile = new Vector3(transform.position.x, transform.position.y + _offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(_healthChangeDisplayContainer.panel, middleOfTheTile, _cam);

        float x = newPosition.x; //  - healthChangeDisplayContainer.resolvedStyle.width / 2; <- 0 on the first frame
        float y = newPosition.y; // - healthChangeDisplayContainer.layout.height; <- height is 0, coz it does not calculate in the first frame after element creation

        _healthChangeDisplayContainer.transform.position = new Vector3(x, y, transform.position.z);
        // move up
        float waitTime = 0;
        float fadeTime = 1f;
        float i = 0.1f;

        while (waitTime < 1f)
        {
            middleOfTheTile = new Vector3(transform.position.x, transform.position.y + _offsetY, transform.position.z);
            newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(_healthChangeDisplayContainer.panel, middleOfTheTile, _cam);

            // TODO: i am calculating x so many times is because size won't be computed on the first frame after creating an element
            // https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
            x = newPosition.x - _healthChangeDisplayContainer.resolvedStyle.width / 2;
            y = newPosition.y - i;
            i += 0.1f;

            _healthChangeDisplayContainer.transform.position = new Vector3(x, y, transform.position.z);

            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }

        _healthChangeDisplayContainer.style.display = DisplayStyle.None;
    }

    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (_coroutineQueue.Count > 0)
                yield return StartCoroutine(_coroutineQueue.Dequeue());
            yield return null;
        }
    }

    public bool IsQueueEmpty()
    {
        return _coroutineQueue.Count == 0;
    }
}
