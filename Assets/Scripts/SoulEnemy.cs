using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject InteractionPanelObject;
    [SerializeField] private GameObject ActionsPanelObject;
    [SerializeField] private SpriteRenderer EnemySpriteRenderer;

    private SpawnPoint _enemyPosition;

    [Header("Button References")]
    [SerializeField] private Button combatButton;
    [Header("Button References")]
    [SerializeField] private Button bowButton;
    [Header("Button References")]
    [SerializeField] private Button swordButton;

    public Button GetCombatButton() => combatButton;
    public Button GetBowButton() => bowButton;
    public Button GetSwordButton() => swordButton;

    private bool isActionPanelOpen = false;

    public bool IsActionPanelOpen {  get { return isActionPanelOpen; } }

    private EnemyWeakness enemyWeakness;
    private EnemyWeakness killingAttackType;

    [SerializeField] private int basePoints = 100; 

    public int GetBasePoints()
    {
      return basePoints;
    }

    public EnemyWeakness GetWeakness()
    {
         return enemyWeakness;
    }
 

    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint, EnemyWeakness weakness)
    {
        EnemySpriteRenderer.sprite = sprite;
        _enemyPosition = spawnPoint;
        enemyWeakness = weakness;
        gameObject.SetActive(true);
    }

    public SpawnPoint GetEnemyPosition()
    {
        return _enemyPosition;
    }

    public GameObject GetEnemyObject()
    {
        return this.gameObject;
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);

        StartCoroutine(SetSelectedBowNextFrame());
    }
    //private void ActiveCombatWithEnemy()
    //{
    //    ActiveInteractionPanel(false);
    //    ActiveActionPanel(true);

    //    // Odrocz ustawienie focusu na kolejny frame
    //    StartCoroutine(SetSelectedBowNextFrame());
    //}

    private IEnumerator SetSelectedBowNextFrame()
    {
        yield return null; // czeka do następnego frame
        EventSystem.current.SetSelectedGameObject(bowButton.gameObject);
    }
    public void CancelCombatWithEnemy()
    {
        ActiveInteractionPanel(true);
        ActiveActionPanel(false);

        EventSystem.current.SetSelectedGameObject(combatButton.gameObject);
    }


    private void ActiveInteractionPanel(bool active)
    {
        InteractionPanelObject.SetActive(active);
    }

    private void ActiveActionPanel(bool active)
    {
        isActionPanelOpen = active;
        ActionsPanelObject.SetActive(active);
    }

    private void UseBow()
    {
        // USE BOW
        killingAttackType = EnemyWeakness.RANGE;
        GameEvents.EnemyKilled?.Invoke(this);
    }


    private void UseSword()
    {
        killingAttackType = EnemyWeakness.MELEE;
        GameEvents.EnemyKilled?.Invoke(this);
        // USE SWORD
    }

    #region OnClicks
    [ContextMenu("click combat")]
    public void Combat_OnClick()
    {
        ActiveCombatWithEnemy();
    }
    [ContextMenu("use bow")]
    public void Bow_OnClick()
    {
        UseBow();
    }
    [ContextMenu("use sword")]
    public void Sword_OnClick()
    {
        UseSword();
    }

    public EnemyWeakness GetKillingAttackType()
    {
        return killingAttackType;
    }

    #endregion
}


public interface IEnemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
    int GetBasePoints();
    EnemyWeakness GetWeakness();  // <-- słabość wroga
    EnemyWeakness GetKillingAttackType(); // <-- czym został zabity
}

public enum EnemyWeakness
{
    MELEE,
    RANGE
}
