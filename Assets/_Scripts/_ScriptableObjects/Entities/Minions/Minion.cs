

using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Minion")]
    public class Minion : EntityMovement
    {
        [Header("Minion")]
        public GameObject ExplosionPrefab;
        public Sound ExplosionSound;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);

            if (Level.Value <= 1) return;

            for (int i = 1; i < Level.Value; i++)
            {
                MaxHealth.LevelUp();
                Armor.LevelUp();
                Speed.LevelUp();
            }

            CurrentHealth.SetValue(MaxHealth.GetValue());
        }
    }
}

