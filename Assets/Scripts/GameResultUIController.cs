using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultUIController : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text resultText;

    private void OnEnable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.GameFinished += OnGameFinished;
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.GameFinished -= OnGameFinished;
        }
    }

    private void OnGameFinished(bool defenderWon)
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = defenderWon ? "Victory" : "Defeat";
        }
    }

    public void RestartLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
