namespace __GAME__.Source.Game.Gameplay.Weapon.Behaviors
{
    public interface IFiringBehavior
    {
        void OnAttackPressed(WeaponPresenter presenter);
        void OnAttackReleased(WeaponPresenter presenter);
        void Tick(WeaponPresenter presenter);
        void Reset();
    }
}
