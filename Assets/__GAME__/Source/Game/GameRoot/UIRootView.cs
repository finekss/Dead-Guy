using UnityEngine;

namespace __GAME__.Source.Game.GameRoot
{
    public class UIRootView: MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Transform _uiSceneContainer;

        private void Awake()
        {
            _loadingScreen.SetActive(false);
        }
        
        public void ShowLoadingScreen() {_loadingScreen.SetActive(true);}
        public void HideLoadingScreen() {_loadingScreen.SetActive(false);}

        public void AttachSceneUI(GameObject sceneUI)
        {
            CleaneSceneUI();
            
            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }

        private void CleaneSceneUI()
        {
            var childCount = _uiSceneContainer.childCount;
            for (var i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }
    }
}