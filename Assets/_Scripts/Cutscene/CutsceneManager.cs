using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;

public class CutsceneManager : Singleton<CutsceneManager>
{
    GameManager _gameManager;
    AudioManager _audioManager;
    CutsceneCameraManager _cameraManager;

    public Cutscene[] AllCutscenes;
    Cutscene _cutscene;

    Label _text;

    [SerializeField] SpriteRenderer[] _pictureRenderers;
    int _activeRendererIndex = 0;

    float _additionalDelay = 1.2f;

    bool _skippingCutscene; // to block clicks

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _cameraManager = (CutsceneCameraManager)BattleCameraManager.Instance;

        var root = GetComponent<UIDocument>().rootVisualElement;
        _text = root.Q<Label>("text");

        _skippingCutscene = false;

        _cutscene = ChooseCutscene();
        RunScene();
    }

    // TODO: this is wrong but for now it is fine.
    Cutscene ChooseCutscene()
    {
        if (_gameManager.PreviousLevel == Scenes.MainMenu)
            return _gameManager.GameDatabase.GetAllCutscenes()[0];
        if (_gameManager.PreviousLevel == Scenes.Rat)
            return _gameManager.GameDatabase.GetAllCutscenes()[1];

        return _gameManager.GameDatabase.GetAllCutscenes()[0];
    }

    async void RunScene()
    {
        _audioManager.PlayMusic(_cutscene.Music);

        await Task.Delay(500);

        foreach (CutscenePicture c in _cutscene.Pictures)
            await DisplayCutscene(c);

        await Task.Delay(1000);
        _gameManager.LoadLevel(_cutscene.NextLevelName);
    }

    async Task DisplayCutscene(CutscenePicture c)
    {
        FadeOutOldPicture(_pictureRenderers[_activeRendererIndex]);

        // swap active renderers
        _activeRendererIndex++;
        if (_activeRendererIndex >= _pictureRenderers.Length)
            _activeRendererIndex = 0;

        FadeInNewPicture(c.Picture, _pictureRenderers[_activeRendererIndex]);

        float duration = c.TextToSpeech.Clips[0].length + _additionalDelay;
        _audioManager.PlayDialogue(c.TextToSpeech);

        _cameraManager.PanCamera(c.CameraPanDirection, duration);
        if (c.ZoomCameraIn)
            _cameraManager.ZoomCameraIn(duration);
        else
            _cameraManager.ZoomCameraOut(duration);

        await PrintText(c.Text, duration);

        await Task.Delay(Mathf.FloorToInt(_additionalDelay * 1000));
    }

    void FadeInNewPicture(Sprite s, SpriteRenderer renderer)
    {
        renderer.sprite = s;
        renderer.color = new Color(1f, 1f, 1f, 0f);
        renderer.DOFade(1f, 2f);
    }

    void FadeOutOldPicture(SpriteRenderer renderer)
    {
        renderer.DOFade(0f, 2f);
    }

    async Task PrintText(string text, float duration)
    {
        _text.text = "";
        char[] charArray = text.ToCharArray();

        duration -= 1f;
        int delay = Mathf.FloorToInt(duration * 1000) / charArray.Length - 10; // magic -10

        for (int i = 0; i < charArray.Length; i++)
        {
            _text.text += charArray[i];
            await Task.Delay(delay);
        }
    }

    public void SkipCutscene()
    {
        if (_skippingCutscene)
            return;
        _skippingCutscene = true;

        AudioManager.Instance.StopDialogue();
        _gameManager.LoadLevel(_cutscene.NextLevelName);
    }
}
