using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 5f; // im wi�ksza warto��, tym szybciej

    private void Awake()
    {
        Instance = this;
    }

    public void MoveToPosition(Vector2 target)
    {
        // Obliczamy czas ruchu na podstawie pr�dko�ci i dystansu
        float distance = Mathf.Abs(target.x - mainCamera.transform.position.x);
        float duration = distance / moveSpeed;

        // Zatrzymujemy poprzednie tweens, je�li dzia�aj�
        mainCamera.transform.DOKill();

        // Ruch tylko po osi X
        mainCamera.transform.DOMoveX(target.x, duration)
            .SetEase(Ease.OutCubic); // mo�esz zmieni� typ easingu
    }
}
