using UnityEngine.UIElements;

namespace Lis.Core
{
    public class VerticalSpacerElement : VisualElement
    {
        private const string _ussCommonSpacer = "common__vertical-spacer";

        public VerticalSpacerElement()
        {
            AddToClassList(_ussCommonSpacer);
        }
    }
}