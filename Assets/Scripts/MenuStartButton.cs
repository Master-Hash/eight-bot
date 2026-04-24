using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MenuStartButton : MonoBehaviour
    {
        [SerializeField] private Toggle playerFirstToggle;
        [SerializeField] private string gameplaySceneName = "Main";

        public void OnStartClicked()
        {
            if (playerFirstToggle != null)
            {
                GameConfig.PlayerFirst = playerFirstToggle.isOn;
                GameConfig.ResetState();
            }
            else
            {
                Debug.LogWarning("Player First toggle is not assigned. Keeping previous GameConfig.PlayerFirst value.");
                GameConfig.ResetState();
            }

            if (string.IsNullOrWhiteSpace(gameplaySceneName))
            {
                Debug.LogError("Gameplay scene name is empty. Cannot load scene.");
                return;
            }

            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
