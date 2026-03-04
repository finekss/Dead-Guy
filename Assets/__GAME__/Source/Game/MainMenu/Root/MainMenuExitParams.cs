using __GAME__.Source.Game.Gameplay.Root;

namespace __GAME__.Source.Game.MainMenu.Root
{
    public class MainMenuExitParams
    {
        public GameplayEntryParams GameplayEntryParams { get; }

        public MainMenuExitParams(GameplayEntryParams gameplayEntryParams)
        {
            GameplayEntryParams =  gameplayEntryParams;
        }
    }
}