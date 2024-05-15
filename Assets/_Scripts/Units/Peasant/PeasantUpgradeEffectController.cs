using Lis.Core;
using UnityEngine;

namespace Lis
{
    public class PeasantUpgradeEffectController : MonoBehaviour
    {
        [SerializeField] ParticleSystem _changeStartColor;
        [SerializeField] ParticleSystem[] _changeGradientColor;

        public void Play(Nature n)
        {
            var ch = _changeStartColor.main;
            ch.startColor = n.Color.Primary;

            foreach (ParticleSystem ps in _changeGradientColor)
            {
                var colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.color = new ParticleSystem.MinMaxGradient(n.Color.Primary, n.Color.Secondary);
            }
        }
    }
}