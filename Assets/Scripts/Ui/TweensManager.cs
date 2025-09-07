using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TweensManager : MonoBehaviour
{
    public static TweensManager Instance { get; private set; }

    [Header("Object References")]
    [SerializeField] private Image hitEffectImage;
    [SerializeField] private TextMeshProUGUI criticalText;

    [Header("Objects sprites and texts")]
    [SerializeField] private Sprite slashSprite;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private Sprite criticalSlashSprite;
    [SerializeField] private Sprite criticalHitSprite;
    [SerializeField] private string criticalTextString="CRITICAL!";

    [Header("Tween values")]
    [SerializeField] private Vector3 hitEffectRange = new Vector3(50f, 50f, 0f);
    [SerializeField] private float maxRotationZ = 20f;

    [SerializeField] private float effectDuration = 0.3f;
    [SerializeField] private float criticalScale = 1.5f;
    [SerializeField] private float criticalDuration = 0.5f;
    [SerializeField] private Vector3 criticalTextOffside;
    private Dictionary<(EnemyWeakness, bool), Sprite> spriteMap;

    private Tween hitTween;
    private Tween criticalTween;
    private Tween punchTween;

    private Camera mainCamera;

    public float EffectDuration { get => effectDuration; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitTweensManager();
    }

    public void InitTweensManager()
    {
        mainCamera = Camera.main;
        criticalText.SetText(criticalTextString);
    }

    private void OnEnable()
    {
        GameEvents.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= OnEnemyKilled;
    }
    private void KillTween(ref Tween tween)
    {
        if (tween != null && tween.IsActive()) tween.Kill();
        tween = null;
    }
    public void OnEnemyKilled(IEnemy enemy)
    {
        Vector3 worldPos = enemy.GetEnemyObject().transform.position;
        EnemyWeakness attackType = enemy.GetKillingAttackType();
        bool isCritical = (enemy.GetWeakness() == attackType);

        Sprite selectedSprite = GetSpriteForHit(attackType, isCritical);
        PlayEffect(worldPos, selectedSprite);

        if (isCritical)
            PlayCriticalText(worldPos);
    }

    private Sprite GetSpriteForHit(EnemyWeakness attackType, bool isCritical)
    {
        if (isCritical)
        {
            return attackType == EnemyWeakness.MELEE ? criticalSlashSprite : criticalHitSprite;
        }
        else
        {
            return attackType == EnemyWeakness.MELEE ? slashSprite : hitSprite;
        }
    }
    private void PlayEffect(Vector3 worldPos, Sprite sprite)
    {
        if (hitEffectImage == null) return;

        hitEffectImage.sprite = sprite;

        Vector3 randomOffset = new Vector3(
            Random.Range(-hitEffectRange.x, hitEffectRange.x),
            Random.Range(-hitEffectRange.y, hitEffectRange.y),
            0f
        );
        hitEffectImage.transform.position = mainCamera.WorldToScreenPoint(worldPos) + randomOffset;

        float randomRotZ = Random.Range(-maxRotationZ, maxRotationZ);
        hitEffectImage.transform.rotation = Quaternion.Euler(0f, 0f, randomRotZ);

        hitEffectImage.gameObject.SetActive(true);

        KillTween(ref hitTween);

        hitEffectImage.transform.localScale = Vector3.zero;
        hitTween = hitEffectImage.transform.DOScale(1f, effectDuration / 2)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                hitEffectImage.transform.DOScale(0f, effectDuration / 2)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => hitEffectImage.gameObject.SetActive(false));
            });
    }
    private void PlayCriticalText(Vector3 worldPos)
    {
        if (criticalText == null) return;

        criticalText.transform.position = mainCamera.WorldToScreenPoint(worldPos) + criticalTextOffside;
        criticalText.gameObject.SetActive(true);

        KillTween(ref criticalTween);


        criticalText.transform.localScale = Vector3.zero;

        criticalTween = criticalText.transform.DOScale(criticalScale, criticalDuration / 2).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                criticalText.transform.DOScale(0, criticalDuration / 2).SetEase(Ease.InBack)
                    .OnComplete(() => criticalText.gameObject.SetActive(false));
            });
    }

    public void PlayPunchEffect(TextMeshProUGUI targetText, float punchScale = 1.3f, float punchDuration = 0.2f)
    {
        if (targetText == null) return;

        KillTween(ref punchTween);

        Vector3 originalScale = Vector3.one;
        targetText.transform.localScale = originalScale;

        punchTween = targetText.transform.DOScale(punchScale, punchDuration / 2)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                targetText.transform.DOScale(originalScale, punchDuration / 2)
                    .SetEase(Ease.InBack);
            });
    }
}
