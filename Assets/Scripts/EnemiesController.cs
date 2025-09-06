using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    [SerializeField] private List<Sprite> AllEnemies;
    // [SerializeField] private List<SpawnPoint> SpawnPoints;
    [SerializeField] private GameObject EnemyPrefab;

    private int _maxEnemies = 3;
    private int _currentEnemies = 0;

  //  [SerializeField]
   // public List<SoulEnemy> ActiveEnemies { get; private set; } = new List<SoulEnemy>();

    [SerializeField]
    private List<SoulEnemySlot> soulEnemySlots;

    public static event Action<SoulEnemy> OnEnemySpawned;

    public void InitializeEnemies()
    {
       // ConfigureEnemiesController();
        SpawnEnemies();


        Debug.Log("EnemiesController zainicjalizowany");
    }

    private void OnEnable()
    {
        AttachListeners();
    }

    private void OnDisable()
    {
        DettachListeners();
    }

    private void AttachListeners()
    {
        GameEvents.EnemyKilled += EnemyKilled;
    }

    private void DettachListeners()
    {
        GameEvents.EnemyKilled -= EnemyKilled;
    }

    //private void EnemyKilled(IEnemy enemy)
    //{
    //    //FreeSpawnPoint(enemy.GetEnemyPosition());
    //    DestroyKilledEnemy(enemy.GetEnemyObject());
    //    StartCoroutine(SpawnEnemyViaCor());
    //}
    private void EnemyKilled(IEnemy enemy)
    {
        SoulEnemySlot slot = GetSlotByEnemy(enemy.GetEnemyObject().GetComponent<SoulEnemy>());
        if (slot != null)
        {
            slot.enemy = null; // zwolnienie slotu
        }

        DestroyKilledEnemy(enemy.GetEnemyObject());

        _currentEnemies = Mathf.Max(0, _currentEnemies - 1); // zmniejszenie licznika
        StartCoroutine(SpawnEnemyViaCor()); // nowy enemy może pojawić się w zwolnionym slocie
    }

    private void SpawnEnemies()
    {
        foreach (var slot in soulEnemySlots)
        {
            if (slot.enemy == null)
                SpawnEnemy();
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
     //   ActiveEnemies.Add(enemy);
        // Focus na nowo stworzonego enemy
        GameControlller.Instance.GameplayInput.FocusEnemy(enemy);

}

    private SoulEnemySlot GetFirstFreeSlot()
    {
        foreach (var slot in soulEnemySlots)
        {
            if (slot.enemy == null)
                return slot;
        }
        return null;
    }



    private SoulEnemySlot GetSlotByEnemy(SoulEnemy enemy)
{
    foreach (var slot in soulEnemySlots)
    {
        if (slot.enemy == enemy) return slot;
    }
    return null;
}

// Pobranie listy wszystkich aktywnych enemy
public List<SoulEnemy> GetActiveEnemies()
{
    List<SoulEnemy> active = new List<SoulEnemy>();
    foreach (var slot in soulEnemySlots)
    {
        if (slot.enemy != null) active.Add(slot.enemy);
    }
    return active;
}

//private void SpawnEnemy()
//{
//    if (_currentEnemies >= _maxEnemies)
//    {
//        Debug.LogError("Max Enemies reached! Kil some to spawn new");
//        return;
//    }

//    int freeSpawnPointIndex = -1;
//    for (int i = 0; i < SpawnPoints.Count; i++)
//    {
//        if (SpawnPoints[i].IsOccupied) continue;

//        freeSpawnPointIndex = i;
//        break;
//    }

//    if (freeSpawnPointIndex == -1) return;

//    SpawnPoints[freeSpawnPointIndex].IsOccupied = true;
//    SoulEnemy enemy = Instantiate(EnemyPrefab, SpawnPoints[freeSpawnPointIndex].Position.position, Quaternion.identity, transform).GetComponent<SoulEnemy>();
//    int spriteIndex = UnityEngine.Random.Range(0, AllEnemies.Count);
//    EnemyWeakness weakness = UnityEngine.Random.value > 0.5f ? EnemyWeakness.MELEE : EnemyWeakness.RANGE;
//    enemy.SetupEnemy(AllEnemies[spriteIndex], SpawnPoints[freeSpawnPointIndex], weakness);

//    ActiveEnemies.Add(enemy);
//    _currentEnemies++;

//    OnEnemySpawned?.Invoke(enemy);

//    GameControlller.Instance.GameplayInput.FocusEnemy(enemy);
//}

private void DestroyKilledEnemy(GameObject enemy)
{
  //  ActiveEnemies.Remove(enemy.GetComponent<SoulEnemy>());
    Destroy(enemy);
}

//private void FreeSpawnPoint(SpawnPoint spawnPoint)
//{
//    for (int i = 0; i < SpawnPoints.Count; i++)
//    {
//        if (spawnPoint != SpawnPoints[i]) continue;

//        SpawnPoints[i].IsOccupied = false;
//        _currentEnemies--;
//        break;
//    }
//}

    //private void ConfigureEnemiesController()
    //{
    //    _maxEnemies = SpawnPoints != null ? SpawnPoints.Count : 3;
    //}

}

[System.Serializable]
public class SpawnPoint
{
    public Transform Position;
    public bool IsOccupied;
}


