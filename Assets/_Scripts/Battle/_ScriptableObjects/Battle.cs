using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
    public class Battle : BaseScriptableObject
    {
        public Stats Stats;

        public Hero SelectedHero;

        public List<Arena.Arena> Arenas;

        public Arena.Arena CurrentArena;

        public void Initialize(int level)
        {
            Stats = CreateInstance<Stats>();

            foreach (Arena.Arena arena in Arenas)
                arena.Initialize(level);

            SetCurrentArena();
            SetRandomHero();
        }

        void SetCurrentArena()
        {
            CurrentArena = Arenas[0];
        }

        void SetRandomHero()
        {
            SelectedHero = Instantiate(GameManager.Instance.UnitDatabase.GetRandomHero());
            SelectedHero.InitializeHero();
        }
    }
}