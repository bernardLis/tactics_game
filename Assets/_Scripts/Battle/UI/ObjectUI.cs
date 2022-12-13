using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Shapes;

public class ObjectUI : ImmediateModeShapeDrawer
{
    Camera _cam;
    BattleManager _battleManager;

    Queue<IEnumerator> _coroutineQueue = new();

    VisualElement _root;

    GameObject _body;
    [SerializeField] bool _isNameDisplayed;
    [SerializeField] bool _blockNameDisplay;
    [SerializeField] string _name;
    [SerializeField] int _fontSize;
    [SerializeField] Color _color;

    [SerializeField] Vector3 _nameLabelOffset = new();

    void Awake()
    {
        _body = GetComponentInChildren<CharacterRendererManager>().gameObject;
    }

    void Start()
    {
        _cam = Helpers.Camera;
        _battleManager = BattleManager.Instance;

        // getting ui elements
        _root = GetComponent<UIDocument>().rootVisualElement;

        DisplayName();

        StartCoroutine(CoroutineCoordinator());
        _battleManager.OnGamePaused += OnGamePaused;
        _battleManager.OnGameResumed += OnGameResumed;
    }


    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
            if (_isNameDisplayed && !_blockNameDisplay && _name != null)
                Draw.Text(_body.transform.position + _nameLabelOffset, _name, _fontSize, _color);
    }


    void OnGamePaused() { _isNameDisplayed = false; }

    void OnGameResumed() { _isNameDisplayed = true; }

    public void ToggleCharacterNameDisplay(bool isDisplayed) { _blockNameDisplay = !isDisplayed; }

    void DisplayName()
    {
        if (TryGetComponent(out CharacterStats stats))
        {
            _isNameDisplayed = true;
            _name = stats.Character.CharacterName;
            _color = Color.white;
            _fontSize = 3;
            _nameLabelOffset = new(0, -0.4f - _body.transform.localPosition.y);

            if (CompareTag(Tags.Enemy))
                _color = Color.red;
        }
    }

    // TODO: I could be displaying some effects on character - poison cloud or something 
    // and add it to the queue as well...
    public void DisplayOnCharacter(string txt, int fontSize, Color col)
    {
        _coroutineQueue.Enqueue(DisplayOnCharacterCoroutine(txt, fontSize, col));
    }

    IEnumerator DisplayOnCharacterCoroutine(string txt, int fontSize, Color col)
    {
        _root.Add(new CombatTextElement(txt, col, transform, _root));
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
