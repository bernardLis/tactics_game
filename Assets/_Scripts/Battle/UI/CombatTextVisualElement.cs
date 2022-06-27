using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class CombatTextVisualElement : VisualElement
{
    float _offsetY = 0.5f;

    Camera _cam;
    VisualElement _root;
    Transform _characterTransform;

    public CombatTextVisualElement(string text, Color col, Transform tr, VisualElement root)
    {
        _cam = Camera.main;
        _root = root;
        _characterTransform = tr;
        Debug.Log($"trpos: {tr.position}");

        Label textLabel = new Label(text);
        textLabel.AddToClassList("primaryText");
        textLabel.style.color = col;
        Add(textLabel);

        OnPostVisualCreation();
    }

    //https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
    void OnPostVisualCreation()
    {
        // Make invisble so you don't see the size re-adjustment
        // (Non-visible objects still go through transforms in the layout engine)
        visible = false;
        schedule.Execute(WaitOneFrame);
    }

    void WaitOneFrame(TimerState obj)
    {
        // Because waiting once wasn't working
        schedule.Execute(AsyncMove);
    }

    async void AsyncMove()
    {
        // Do any measurements, size adjustments you need (NaNs not an issue now)
        MarkDirtyRepaint();
        visible = true;
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(this.panel, _characterTransform.position, _cam);
        transform.position = new Vector3(newPosition.x, newPosition.y + _offsetY, transform.position.z);
        Debug.Log($"transform.position: {transform.position}");

        Debug.Log("asyncMove");
        await Move();
    }

    async Task Move()
    {
        Debug.Log("move");

        // set position of the element 
        Vector3 middleOfTheTile;
        Vector2 newPosition;
        float x;
        float y;

        // move up
        float waitTime = 0;
        float fadeTime = 10f;
        float i = 0.01f;

        while (waitTime < 10f)
        {
            middleOfTheTile = new Vector3(_characterTransform.position.x, _characterTransform.position.y + _offsetY, _characterTransform.position.z);
            newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(panel, middleOfTheTile, _cam);
            Debug.Log($"new position: {newPosition}");

            x = newPosition.x;
            y = newPosition.y - i;
            i += 0.01f;

            transform.position = new Vector3(x, y, transform.position.z);

            await Task.Yield();
            waitTime += Time.deltaTime / fadeTime;
        }

        //Hide();
    }

    void Hide()
    {
        this.SetEnabled(false);
        _root.Remove(this);
    }


}
