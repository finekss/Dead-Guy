namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public static class FiringBehaviorFactory
    {
        public static IFiringBehavior Create(FiringMode firingMode)
        {
            return firingMode switch
            {
                FiringMode.SemiAuto => new SemiAutoFiringBehavior(),
                FiringMode.Automatic => new AutomaticFiringBehavior(),
                FiringMode.Charged => new ChargedFiringBehavior(),
                FiringMode.Melee => new MeleeFiringBehavior(),
                FiringMode.Beam => new BeamFiringBehavior(),
                _ => new SemiAutoFiringBehavior()
            };
        }
    }
}
