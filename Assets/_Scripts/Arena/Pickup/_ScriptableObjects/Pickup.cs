using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Arena.Pickup
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

        public virtual void HandleHeroBonuses(Hero hero)
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