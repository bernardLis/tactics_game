using Lis.Core;
using UnityEngine;

namespace Lis
{
    public class PeasantUpgradeEffectController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _changeStartColor;
        [SerializeField] private ParticleSystem[] _changeGradientColor;

        public void Play(Nature n)
        {
            ParticleSystem.MainModule ch = _changeStartColor.main;
            ch.startColor = n.Color.Primary;

            foreach (ParticleSystem ps in _changeGradientColor)
            {
                ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.color = new(n.Color.Primary, n.Color.Secondary);
            }
        }
    }
}