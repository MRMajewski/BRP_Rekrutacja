using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 5f; // im wiêksza wartoœæ, tym szybciej

    private void Awake()
    {
        Instance = this;
    }

    public void MoveToPosition(Vector2 target)
    {
        // Obliczamy czas ruchu na podstawie prêdkoœci i dystansu
        float distance = Mathf.Abs(target.x - mainCamera.transform.position.x);
        float duration = distance / moveSpeed;

        // Zatrzymujemy poprzednie tweens, jeœli dzia³aj¹
        mainCamera.transform.DOKill();

        // Ruch tylko po osi X
        mainCamera.transform.DOMoveX(target.x, duration)
            .SetEase(Ease.OutCubic); // mo¿esz zmieniæ typ easingu
    }
}
