



namespace Lis
{
    public class TurretIcon : ElementWithTooltip
    {
        const string _ussClassName = "turret-icon__";
        const string _ussMain = _ussClassName + "main";

        readonly GameManager _gameManager;

        readonly Turret _turret;
        readonly bool _blockTooltip;
        public TurretIcon(Turret turret, bool blockTooltip = false)
        {
            _gameManager = GameManager.Instance;
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TurretIconStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _turret = turret;
            _blockTooltip = blockTooltip;

            AddToClassList(_ussMain);
            style.backgroundImage = turret.Icon.texture;
        }


        protected override void DisplayTooltip()
        {
            if (_blockTooltip) return;

            TurretCard tc = new(_turret);

            _tooltip = new(this, tc);
            base.DisplayTooltip();
        }

    }
}
