using __GAME__.Source.Game.MainMenu.Root;

namespace __GAME__.Source.Game.Gameplay.Root
{
    public class GameplayExitParams
    {
        public MainMenuEntryParams MainMenuEntryParams { get; }

        public GameplayExitParams(MainMenuEntryParams mainMenuEntryParams)
        {
            MainMenuEntryParams = mainMenuEntryParams;
        }
    }
}