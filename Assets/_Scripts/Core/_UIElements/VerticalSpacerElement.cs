using UnityEngine.UIElements;

namespace Lis.Core
{
    public class VerticalSpacerElement : VisualElement
    {
        const string _ussCommonSpacer = "common__vertical-spacer";

        public VerticalSpacerElement()
        {
            AddToClassList(_ussCommonSpacer);
        }
    }
}