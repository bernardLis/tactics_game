using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class BattleGrabbableObstacle : MonoBehaviour, IPointerDownHandler
{
    BattleGrabManager _grabManager;
    bool _wasGrabbed;
    MMF_Player _feelPlayer;
    Color _grabbedColor = new Color(0.875f, 0.32f, 0.28f, 1f); // reddish

    int _secondsToBreak = 3;
    void Start()
    {
        _grabManager = BattleGrabManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_secondsToBreak <= 0)
        {
            DisplayText("No more grabbing!", Color.red);
            return;
        }
        if (!_grabManager.IsGrabbingAllowed()) return;

        _grabManager.TryGrabbing(gameObject);

        StartCoroutine(GrabBreaker());
    }

    public void Released()
    {
        StopAllCoroutines();
        DisplayText("Released!", Color.red);
        DOTween.Kill("GrabbedColor");
    }

    IEnumerator GrabBreaker()
    {
        Material mat = GetComponent<Renderer>().material;
        mat.DOColor(_grabbedColor, _secondsToBreak).SetId("GrabbedColor");

        for (int i = _secondsToBreak; i > 0; i--)
        {
            _secondsToBreak--;
            DisplayText($"{i}", Color.red);
            yield return new WaitForSeconds(1f);
        }
        DisplayText("Release!", Color.red);
        _grabManager.OnPointerUp(default);
    }

    void DisplayText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
        _feelPlayer.PlayFeedbacks(pos);
    }

}
