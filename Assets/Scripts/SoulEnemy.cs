using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulEnemy : MonoBehaviour, IEnemy
{

    #region Variables
    [Header("Object References")]
    [SerializeField] private GameObject InteractionPanelObject;
    [SerializeField] private GameObject ActionsPanelObject;
    [SerializeField] private SpriteRenderer EnemySpriteRenderer;

    [Header("Button References")]
    [SerializeField] private Button combatButton;
    [SerializeField] private Button bowButton;
    [SerializeField] private Button swordButton;

    [Header("Enemy Settings")]
    [SerializeField] private int basePoints = 100;


    private SpawnPoint _enemyPosition;
    private EnemyWeakness enemyWeakness;
    private EnemyWeakness killingAttackType;
    private bool isActionPanelOpen = false;
    private bool isDead = false;


    public bool IsActionPanelOpen => isActionPanelOpen;


    public Button GetCombatButton() => combatButton;
    public Button GetBowButton() => bowButton;
    public Button GetSwordButton() => swordButton;


    public int GetBasePoints() => basePoints;
    public EnemyWeakness GetWeakness() => enemyWeakness;
    public SpawnPoint GetEnemyPosition() => _enemyPosition;
    public GameObject GetEnemyObject() => gameObject;
    public EnemyWeakness GetKillingAttackType() => killingAttackType;

    #endregion

    #region Setup
    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint, EnemyWeakness weakness)
    {
        EnemySpriteRenderer.sprite = sprite;
        _enemyPosition = spawnPoint;
        enemyWeakness = weakness;
        gameObject.SetActive(true);
    }
    #endregion

    #region Combat
    public void Combat_OnClick() => ActiveCombatWithEnemy();
    public void Bow_OnClick() => UseWeapon(EnemyWeakness.RANGE);
    public void Sword_OnClick() => UseWeapon(EnemyWeakness.MELEE);
    private void UseWeapon(EnemyWeakness attackType)
    {
        if (isDead) return;
        killingAttackType = attackType;
        isDead = true;
        GameEvents.EnemyKilled?.Invoke(this);
    }
    public void CancelCombatWithEnemy()
    {
        ActiveInteractionPanel(true);
        ActiveActionPanel(false);
        EventSystem.current.SetSelectedGameObject(combatButton.gameObject);
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);
        StartCoroutine(SetSelectedBowNextFrame());
    }

    private IEnumerator SetSelectedBowNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(bowButton.gameObject);
    }

    private void ActiveInteractionPanel(bool active) => InteractionPanelObject.SetActive(active);
    private void ActiveActionPanel(bool active)
    {
        isActionPanelOpen = active;
        ActionsPanelObject.SetActive(active);
    }
    #endregion

}

public interface IEnemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
    int GetBasePoints();
    EnemyWeakness GetWeakness();
    EnemyWeakness GetKillingAttackType();
}

public enum EnemyWeakness
{
    MELEE,
    RANGE
}
