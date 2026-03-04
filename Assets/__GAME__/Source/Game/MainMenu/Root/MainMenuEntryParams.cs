
namespace __GAME__.Source.Game.MainMenu.Root
{
    public class MainMenuEntryParams
    {
        public bool IsSaveData { get; }
        
        public MainMenuEntryParams(bool isSaveData = false)
        {
            IsSaveData = isSaveData;
        }
    }
}