
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
    public class Battle : BaseScriptableObject
    {
        GameManager _gameManager;

        public int Duration = 1200; // seconds
        public int TilesUntilBoss = 3;

        public bool Won;

        public void CreateRandom(int level)
        {
            _gameManager = GameManager.Instance;
        }
    }
}