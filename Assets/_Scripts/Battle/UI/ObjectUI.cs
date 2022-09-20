using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectUI : MonoBehaviour
{
    Camera _cam;
    BattleManager _battleManager;

    Queue<IEnumerator> _coroutineQueue = new();

    VisualElement _root;
    Label _characterNameLabel;
    bool _isNameDisplayed;

    [SerializeField] float _offsetY = -0.25f;
    [SerializeField] float _offsetX = -0.25f;
    void Start()
    {
        _cam = Helpers.Camera;
        _battleManager = BattleManager.Instance;

        // getting ui elements
        _root = GetComponent<UIDocument>().rootVisualElement;
        _characterNameLabel = _root.Q<Label>("characterName");

        SetNameLabelColor();
        DisplayName();

        StartCoroutine(CoroutineCoordinator());
        _battleManager.OnGamePaused += OnGamePaused;
        _battleManager.OnGameResumed += OnGameResumed;

    }

    void OnGamePaused() { _characterNameLabel.style.display = DisplayStyle.None; }

    void OnGameResumed() { _characterNameLabel.style.display = DisplayStyle.Flex; }

    void SetNameLabelColor()
    {
        if (CompareTag(Tags.Enemy))
            _characterNameLabel.style.color = Color.red;
    }

    void DisplayName()
    {
        if (TryGetComponent(out CharacterStats stats))
        {
            _characterNameLabel.style.display = DisplayStyle.Flex;
            _characterNameLabel.style.alignSelf = Align.FlexStart;
            _characterNameLabel.text = stats.Character.CharacterName;
            // TODO: this is imperfect coz it depends on the camera zoom really...
            _offsetX = stats.Character.CharacterName.Length * -0.06f; // magic number!! 
            _isNameDisplayed = true;
        }

    }

    void Update()
    {
        if (_isNameDisplayed)
            CenterNameLabel();
    }

    void CenterNameLabel()
    {

        Vector3 middleOfTheTile = new Vector3(transform.position.x + _offsetX, transform.position.y + _offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(_characterNameLabel.panel, middleOfTheTile, _cam);

        _characterNameLabel.transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
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
