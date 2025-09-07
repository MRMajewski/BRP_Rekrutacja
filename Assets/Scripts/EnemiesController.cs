using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    #region Variables & Events
    [SerializeField] private List<Sprite> AllEnemies;
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private List<SoulEnemySlot> soulEnemySlots;

    private int _currentEnemies = 0;
    public static event Action<SoulEnemy> OnEnemySpawned;
    #endregion

    #region Initialization & Listeners
    public void InitializeEnemies()
    {
        SpawnEnemies();
        Debug.Log("EnemiesController zainicjalizowany");
    }

    private void OnEnable() => AttachListeners();
    private void OnDisable() => DettachListeners();

    private void AttachListeners()
    {
        GameEvents.EnemyKilled += EnemyKilled;
    }

    private void DettachListeners()
    {
        GameEvents.EnemyKilled -= EnemyKilled;
    }
    #endregion

    #region Enemy Handling
    private void EnemyKilled(IEnemy enemy)
    {
        StartCoroutine(HandleEnemyKilled(enemy));
    }

    private IEnumerator HandleEnemyKilled(IEnemy enemy)
    {
        yield return new WaitForSecondsRealtime(TweensManager.Instance.EffectDuration + .3f);

        SoulEnemySlot slot = GetSlotByEnemy(enemy.GetEnemyObject().GetComponent<SoulEnemy>());
        if (slot != null) slot.enemy = null;

        DestroyKilledEnemy(enemy.GetEnemyObject());

        _currentEnemies = Mathf.Max(0, _currentEnemies - 1);
        StartCoroutine(SpawnEnemyViaCor());
    }

    private void SpawnEnemies()
    {
        foreach (var slot in soulEnemySlots)
        {
            if (slot.enemy == null) SpawnEnemy();
        }
    }

    private IEnumerator SpawnEnemyViaCor()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        SoulEnemySlot freeSlot = GetFirstFreeSlot();
        if (freeSlot == null)
        {
            Debug.LogWarning("Brak wolnych slotów!");
            return;
        }

        SoulEnemy enemy = Instantiate(EnemyPrefab, freeSlot.spawnPoint.Position.position, Quaternion.identity, transform)
            .GetComponent<SoulEnemy>();

        int spriteIndex = UnityEngine.Random.Range(0, AllEnemies.Count);
        EnemyWeakness weakness = UnityEngine.Random.value > 0.5f ? EnemyWeakness.MELEE : EnemyWeakness.RANGE;

        enemy.SetupEnemy(AllEnemies[spriteIndex], freeSlot.spawnPoint, weakness);

        freeSlot.enemy = enemy;
        _currentEnemies++;

        OnEnemySpawned?.Invoke(enemy);
        GameControlller.Instance.GameplayInput.FocusEnemy(enemy);
    }

    private SoulEnemySlot GetFirstFreeSlot()
    {
        foreach (var slot in soulEnemySlots)
        {
            if (slot.enemy == null) return slot;
        }
        return null;
    }

    public List<SoulEnemySlot> GetAllSoulEnemySlots() => soulEnemySlots ?? null;

    private SoulEnemySlot GetSlotByEnemy(SoulEnemy enemy)
    {
        foreach (var slot in soulEnemySlots)
        {
            if (slot.enemy == enemy) return slot;
        }
        return null;
    }

    public List<SoulEnemy> GetActiveEnemies()
    {
        List<SoulEnemy> active = new List<SoulEnemy>();
        foreach (var slot in soulEnemySlots)
        {
            if (slot.enemy != null) active.Add(slot.enemy);
        }
        return active;
    }

    private void DestroyKilledEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }
    #endregion
}

[System.Serializable]
public class SpawnPoint
{
    public Transform Position;
    public bool IsOccupied;
}
