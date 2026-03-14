using UnityEngine;
using TMPro;

public class StartMenuUIController : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TowerBuildController towerBuildController;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text soundToggleLabel;

    private void Start()
    {
        Time.timeScale = 1f;

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.PlayMainMenuMusic();
        }

        RefreshSoundLabel();
    }

    public void StartGame()
    {
        if (towerBuildController != null)
        {
            towerBuildController.BlockBuildUntilPointerRelease();
        }

        if (enemySpawner != null)
        {
            enemySpawner.StartGame();
        }

        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.StopMusic();
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        return;
#endif
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Time.timeScale = 0f;
            Application.OpenURL("about:blank");
            return;
        }

        Application.Quit();
    }

    public void ToggleSound()
    {
        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.SetMuted(!GameAudio.Instance.IsMuted);
        }
        else
        {
            AudioListener.pause = !AudioListener.pause;
        }

        RefreshSoundLabel();
    }

    private void RefreshSoundLabel()
    {
        if (soundToggleLabel == null)
        {
            return;
        }

        bool muted = GameAudio.Instance != null ? GameAudio.Instance.IsMuted : AudioListener.pause;
        soundToggleLabel.text = muted ? "Sound: Off" : "Sound: On";
    }
}
