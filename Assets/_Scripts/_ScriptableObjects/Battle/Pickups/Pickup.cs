using Lis.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class Pickup : BaseScriptableObject
    {
        public Sprite Icon;
        [FormerlySerializedAs("GFX")] public GameObject Gfx;

        public ColorVariable Color;
        public string CollectedText;
        public GameObject CollectEffect;

        public Sound DropSound;
        public Sound CollectSound;

        public virtual void Initialize()
        {
        }

        public virtual void Collected(Hero hero)
        {
        }

        public virtual string GetCollectedText()
        {
            return CollectedText;
        }
    }
}