using Lis.Core.Utilities;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Lis.Camp
{
    public class HeroCampController : Singleton<HeroCampController>
    {
        MMF_Player _feelPlayer;

        public void Start()
        {
            _feelPlayer = GetComponent<MMF_Player>();
        }

        public void DisplayFloatingText(string text, Color color)
        {
            if (_feelPlayer == null) return;
            MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
            floatingText.Value = text;
            floatingText.ForceColor = true;
            floatingText.AnimateColorGradient = Helpers.GetGradient(color);
            Transform t = transform;
            _feelPlayer.PlayFeedbacks(t.position + t.localScale.y * Vector3.up);
        }
    }
}