using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;

public class CutsceneManager : MonoBehaviour
{
    CutsceneCameraManager _cameraManager;

    [SerializeField] Cutscene[] _cutscenes;

    Label _text;

    SpriteRenderer _pictureRenderer;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _text = root.Q<Label>("text");

        _pictureRenderer = GetComponentInChildren<SpriteRenderer>();

        _cameraManager = (CutsceneCameraManager)CameraManager.Instance;

        RunScene();
    }

    async void RunScene()
    {
        Debug.Log("run scene");
        foreach (Cutscene c in _cutscenes)
        {
            DisplayCutscene(c);
            // TODO: print text
            await Task.Delay(c.DisplayDuration * 1000);
        }
    }

    void DisplayCutscene(Cutscene c)
    {
        Debug.Log($"DisplayCutscene {c.name}");

        _pictureRenderer.sprite = c.Picture;
        _text.text = c.Text;

        _cameraManager.PanCamera(c.CameraPanDirection, c.DisplayDuration);
        if (c.ZoomCameraIn)
            _cameraManager.ZoomCameraIn(c.DisplayDuration);
        else
            _cameraManager.ZoomCameraOut(c.DisplayDuration);
    }


}
