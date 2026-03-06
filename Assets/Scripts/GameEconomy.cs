using UnityEngine;
using System;

public class GameEconomy : MonoBehaviour
{
    [SerializeField] private int startGold = 300;

    public event Action<int> GoldChanged;

    public int CurrentGold { get; private set; }

    private void Awake()
    {
        CurrentGold = startGold;
        Debug.Log($"Gold: {CurrentGold}");
        GoldChanged?.Invoke(CurrentGold);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (CurrentGold < amount)
        {
            return false;
        }

        CurrentGold -= amount;
        Debug.Log($"Gold: {CurrentGold}");
        GoldChanged?.Invoke(CurrentGold);
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        CurrentGold += amount;
        Debug.Log($"Gold: {CurrentGold}");
        GoldChanged?.Invoke(CurrentGold);
    }
}
