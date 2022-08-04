using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectUI : MonoBehaviour
{
    Camera _cam;

    Queue<IEnumerator> _coroutineQueue = new();

    VisualElement _root;

    void Start()
    {
        _cam = Helpers.Camera;

        // getting ui elements
        _root = GetComponent<UIDocument>().rootVisualElement;
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
        _root.Add(new CombatTextVisualElement(txt, col, transform, _root));
        yield return new WaitForSeconds(1);
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

    public bool IsQueueEmpty() { return _coroutineQueue.Count == 0; }
}
