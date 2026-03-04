namespace __GAME__.Source.Game.Gameplay.Root
{
    public partial class GameplayEntryParams
    {
        public int CurrentHealth { get; }
        public int MaxHealth  { get; }
        public string FirstWeaponID { get; }
        public string SecondWeaponID { get; }

        public GameplayEntryParams(int currentHealth, int maxHealth, string firstWeaponID, string secondWeaponID)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            FirstWeaponID = firstWeaponID;
            SecondWeaponID = secondWeaponID;
        }

    }
}