using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public delegate void OnMoneyUpEventHandler();
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public int MonsterCount;
    public int Money = 50;
    public int SummonCount = 20;

    public event OnMoneyUpEventHandler OnMoneyUp;

    public List<Monster> monsters = new List<Monster>();

    public void GetMoney(int value)
    {
        Money += value;
        OnMoneyUp?.Invoke();
    }

    public void AddMonster(Monster monster)
    {
        monsters.Add(monster);
    }

    public void RemoveMonster(Monster monster)
    {
        monsters.Remove(monster);
    }
}
