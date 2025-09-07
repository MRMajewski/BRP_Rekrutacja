using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 5f; 

    private void Awake()
    {
        Instance = this;
    }

    public void MoveToPosition(Vector2 target)
    {
        float distance = Mathf.Abs(target.x - mainCamera.transform.position.x);
        float duration = distance / moveSpeed;

        mainCamera.transform.DOKill();

        mainCamera.transform.DOMoveX(target.x, duration)
            .SetEase(Ease.OutCubic); 
    }
}
