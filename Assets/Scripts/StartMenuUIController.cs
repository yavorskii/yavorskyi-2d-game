using UnityEngine;

public class StartMenuUIController : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TowerBuildController towerBuildController;
    [SerializeField] private GameObject panelRoot;

    private void Start()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
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

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }
}
