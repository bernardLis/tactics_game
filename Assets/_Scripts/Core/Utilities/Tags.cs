namespace Lis.Core.Utilities
{
    public static class Tags
    {
        public static string Player
        {
            get => "Player";
            private set { }
        }

        public static int PlayerLayer
        {
            get => 8;
            private set { }
        }


        public static int BattleFloorLayer
        {
            get => 6;
            private set { }
        }

        public static int UnpassableLayer
        {
            get => 3;
            private set { }
        }

        public static int BattleInteractableLayer
        {
            get => 9;
            private set { }
        }


        public static int UIVFXLayer
        {
            get => 7;
            private set { }
        }
    }
}