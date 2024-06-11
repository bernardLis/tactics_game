using UnityEngine;

namespace Lis.Core
{
    [CreateAssetMenu]
    public class StringVariable : ScriptableObject
    {
        [SerializeField]
        private string value = "";

        public string Value
        {
            get => value;
            set => this.value = value;
        }
    }
}