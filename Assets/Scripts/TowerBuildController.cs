using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[System.Serializable]
public class TowerBuildOption
{
    public TowerData towerData;
    public GameObject towerPrefab;
}

public class TowerBuildController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<TowerBuildOption> buildOptions = new();
    [SerializeField] private GameEconomy economy;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaypointPath path;
    [SerializeField] private int selectedOptionIndex;
    [SerializeField] private Vector2 gridOrigin = new(-7f, -4f);
    [SerializeField] private Vector2Int gridSize = new(14, 8);
    [SerializeField] private float cellSize = 1f;

    private readonly HashSet<Vector2Int> occupiedCells = new();
    private readonly HashSet<Vector2Int> blockedPathCells = new();
    private bool blockBuildUntilMouseRelease;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        RebuildPathCells();
    }

    private void Update()
    {
        HandleSelectionInput();

        if (blockBuildUntilMouseRelease)
        {
            if (!IsPrimaryPointerPressed())
            {
                blockBuildUntilMouseRelease = false;
            }

            return;
        }

        if (IsBuildClickPressed())
        {
            TryBuildFromMouse();
        }
    }

    public void BlockBuildUntilPointerRelease()
    {
        blockBuildUntilMouseRelease = true;
    }

    private bool IsBuildClickPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null)
        {
            return false;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        return Mouse.current.leftButton.wasPressedThisFrame;
#else
        return false;
#endif
    }

    private bool IsPrimaryPointerPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.isPressed;
#else
        return false;
#endif
    }

    private void TryBuildFromMouse()
    {
        if (enemySpawner != null && enemySpawner.CurrentPhase != RoundPhase.Preparation)
        {
            return;
        }

        if (mainCamera == null || economy == null)
        {
            Debug.LogError("TowerBuildController: Assign mainCamera and economy.");
            return;
        }

        TowerBuildOption option = GetSelectedOption();
        if (option == null || option.towerData == null || option.towerPrefab == null)
        {
            Debug.LogError("TowerBuildController: Selected build option is invalid.");
            return;
        }

        Vector3 mousePosition = GetMouseScreenPosition();
        Vector3 world = mainCamera.ScreenToWorldPoint(mousePosition);
        world.z = 0f;

        Vector2Int cell = WorldToCell(world);
        if (!IsInBounds(cell))
        {
            return;
        }

        if (blockedPathCells.Contains(cell))
        {
            Debug.Log("Cannot build on path.");
            return;
        }

        if (occupiedCells.Contains(cell))
        {
            Debug.Log("Cell is already occupied.");
            return;
        }

        if (!economy.TrySpendGold(option.towerData.cost))
        {
            Debug.Log("Not enough gold.");
            return;
        }

        Vector3 buildPosition = CellToWorld(cell);
        Instantiate(option.towerPrefab, buildPosition, Quaternion.identity);
        occupiedCells.Add(cell);
    }

    private TowerBuildOption GetSelectedOption()
    {
        if (buildOptions == null || buildOptions.Count == 0)
        {
            return null;
        }

        if (selectedOptionIndex < 0 || selectedOptionIndex >= buildOptions.Count)
        {
            selectedOptionIndex = 0;
        }

        return buildOptions[selectedOptionIndex];
    }

    private void HandleSelectionInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null || buildOptions == null || buildOptions.Count == 0)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            selectedOptionIndex = Mathf.Clamp(0, 0, buildOptions.Count - 1);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            selectedOptionIndex = Mathf.Clamp(1, 0, buildOptions.Count - 1);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            selectedOptionIndex = Mathf.Clamp(2, 0, buildOptions.Count - 1);
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            selectedOptionIndex = Mathf.Clamp(3, 0, buildOptions.Count - 1);
        }
#endif
    }

    private Vector3 GetMouseScreenPosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            return new Vector3(pos.x, pos.y, 0f);
        }

        return Vector3.zero;
#else
        return Vector3.zero;
#endif
    }

    private void RebuildPathCells()
    {
        blockedPathCells.Clear();

        if (path == null || path.Waypoints.Count == 0)
        {
            return;
        }

        for (int i = 0; i < path.Waypoints.Count; i++)
        {
            Transform wp = path.Waypoints[i];
            if (wp == null)
            {
                continue;
            }

            Vector2Int cell = WorldToCell(wp.position);
            if (IsInBounds(cell))
            {
                blockedPathCells.Add(cell);
            }

            if (i == path.Waypoints.Count - 1 || path.Waypoints[i + 1] == null)
            {
                continue;
            }

            Vector2Int nextCell = WorldToCell(path.Waypoints[i + 1].position);
            AddLineCells(cell, nextCell);
        }
    }

    private void AddLineCells(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        int sx = from.x < to.x ? 1 : -1;
        int sy = from.y < to.y ? 1 : -1;
        int err = dx - dy;

        int x = from.x;
        int y = from.y;

        while (true)
        {
            Vector2Int cell = new(x, y);
            if (IsInBounds(cell))
            {
                blockedPathCells.Add(cell);
            }

            if (x == to.x && y == to.y)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

    private Vector2Int WorldToCell(Vector3 world)
    {
        int x = Mathf.FloorToInt((world.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((world.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector3 CellToWorld(Vector2Int cell)
    {
        float x = gridOrigin.x + (cell.x + 0.5f) * cellSize;
        float y = gridOrigin.y + (cell.y + 0.5f) * cellSize;
        return new Vector3(x, y, 0f);
    }

    private bool IsInBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.y >= 0 && cell.x < gridSize.x && cell.y < gridSize.y;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 center = CellToWorld(new Vector2Int(x, y));
                Vector3 size = new(cellSize, cellSize, 0f);
                Gizmos.DrawWireCube(center, size);
            }
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.45f);
        foreach (Vector2Int blocked in blockedPathCells)
        {
            Vector3 center = CellToWorld(blocked);
            Vector3 size = new(cellSize, cellSize, 0f);
            Gizmos.DrawCube(center, size);
        }
    }
}
