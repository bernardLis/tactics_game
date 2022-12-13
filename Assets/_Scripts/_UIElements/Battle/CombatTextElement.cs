using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class CombatTextElement : VisualElement
{
    Camera _cam;
    VisualElement _root;

    float _offsetY = 0.5f;

    Vector3 _middleOfTheTile;

    public CombatTextElement(string text, Color col, Transform tr, VisualElement root)
    {
        _cam = Camera.main;
        _root = root;
        _middleOfTheTile = new Vector3(tr.position.x, tr.position.y + _offsetY, tr.position.z);

        Label textLabel = new Label(text);
        textLabel.AddToClassList("textPrimary");
        textLabel.style.color = col;
        style.position = Position.Absolute;
        Add(textLabel);
        OnPostVisualCreation(); // it is necessary, otherwise null reference error on panel
    }

    //https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
    void OnPostVisualCreation()
    {
        // Make invisible so you don't see the size re-adjustment
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

        UpdatePosition();
        await Move();
        await FadeOut();
        DestroySelf();
    }

    async void UpdatePosition()
    {
        while (this.enabledInHierarchy)
        {
            if (panel == null)
                return;

            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(panel, _middleOfTheTile, _cam);
            transform.position = new Vector3(newPosition.x, newPosition.y - _offsetY);
            await Task.Yield();
        }
    }

    async Task Move()
    {
        int endOffsetY = Random.Range(70, 90);
        await DOTween.To(x => _offsetY = x, 0.5f, endOffsetY, 1).SetEase(Ease.OutSine).AsyncWaitForCompletion();
    }

    async Task FadeOut()
    {
        await DOTween.To(x => style.opacity = x, 1, 0, 1).AsyncWaitForCompletion();
    }

    void DestroySelf()
    {
        this.SetEnabled(false);
        _root.Remove(this);
    }
}
