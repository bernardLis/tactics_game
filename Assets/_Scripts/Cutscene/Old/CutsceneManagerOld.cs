using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class CutsceneManagerOld : Singleton<CutsceneManagerOld>
{
    GameManager _gameManager;
    AudioManager _audioManager;
    CutsceneCameraManagerOld _cameraManager;

    Cutscene _cutscene;

    Label _text;

    [SerializeField] SpriteRenderer[] _pictureRenderers;
    int _activeRendererIndex = 0;

    float _additionalDelay = 1.2f;

    bool _skippingCutscene; // to block clicks

    IEnumerator _mainCoroutine;
    IEnumerator _pictureCoroutine;
    IEnumerator _printTextCoroutine;

    protected override void Awake() { base.Awake(); }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _cameraManager = (CutsceneCameraManagerOld)CutsceneCameraManagerOld.Instance;

        var root = GetComponent<UIDocument>().rootVisualElement;
        _text = root.Q<Label>("text");

        _skippingCutscene = false;

        // _cutscene = ChooseCutscene();
        _mainCoroutine = RunScene();
        StartCoroutine(_mainCoroutine);
    }

    // TODO: this is wrong but for now it is fine.
    // Cutscene ChooseCutscene()
    //  {
    //    Debug.Log($"Choosing a cutscene");
    //return _gameManager.GameDatabase.GetAllCutscenes()[_gameManager.CutsceneIndexToPlay];
    //  }

    IEnumerator RunScene()
    {
        Debug.Log($"Running cutscene coroutine for {_cutscene.name}.");
        _audioManager.PlayMusic(_cutscene.Music);

        yield return new WaitForSeconds(0.5f);

        foreach (CutscenePicture c in _cutscene.Pictures)
        {
            _pictureCoroutine = DisplayCutscene(c);
            yield return _pictureCoroutine;
        }

        if (_skippingCutscene)
            yield break;

        yield return new WaitForSeconds(1f);
        _gameManager.LoadLevel(_cutscene.NextLevelName);
    }

    IEnumerator DisplayCutscene(CutscenePicture c)
    {
        Debug.Log($"Displaying picture {c.name}.");
        if (_skippingCutscene)
            yield break;

        FadeOutOldPicture(_pictureRenderers[_activeRendererIndex]);

        // swap active renderers
        _activeRendererIndex++;
        if (_activeRendererIndex >= _pictureRenderers.Length)
            _activeRendererIndex = 0;

        FadeInNewPicture(c.Picture, _pictureRenderers[_activeRendererIndex]);

        float duration = c.Duration;
        if (c.TextToSpeech != null)
        {
            duration = c.TextToSpeech.Clips[0].length + _additionalDelay;
            _audioManager.PlayDialogue(c.TextToSpeech);
        }

        if (_cameraManager == null)
            yield break;

        _cameraManager.PanCamera(c.CameraPanDirection, duration);
        if (c.ZoomCameraIn)
            _cameraManager.ZoomCameraIn(duration);
        else
            _cameraManager.ZoomCameraOut(duration);

        _printTextCoroutine = PrintText(c.Text, duration);
        yield return _printTextCoroutine;
        yield return new WaitForSeconds(_additionalDelay);
    }

    void FadeInNewPicture(Sprite s, SpriteRenderer renderer)
    {
        if (renderer == null)
            return;
        renderer.sprite = s;
        renderer.color = new Color(1f, 1f, 1f, 0f);
        renderer.DOFade(1f, 2f);
    }

    void FadeOutOldPicture(SpriteRenderer renderer) { renderer.DOFade(0f, 2f); }

    IEnumerator PrintText(string text, float duration)
    {
        Debug.Log($"Printing text for {duration} seconds...");
        _text.text = "";
        char[] charArray = text.ToCharArray();

        float delay = duration / charArray.Length;
        for (int i = 0; i < charArray.Length; i++)
        {
            _text.text += charArray[i];
            yield return new WaitForSeconds(delay);
        }
    }

    public void SkipCutscene()
    {
        Debug.Log($"Skip cutscene invoked.");
        if (_skippingCutscene)
            return;
        _skippingCutscene = true;

        AudioManager.Instance.StopDialogue();
        _gameManager.LoadLevel(_cutscene.NextLevelName);
    }
}
