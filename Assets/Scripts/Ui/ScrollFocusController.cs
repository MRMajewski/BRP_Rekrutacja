using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollFocusController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollDuration = 0.25f;

    public void EnsureSelectedVisible(RectTransform target)
    {
        if (target == null) return;

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // Światowe współrzędne targetu i viewportu
        Vector3[] targetCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        target.GetWorldCorners(targetCorners);
        viewport.GetWorldCorners(viewportCorners);

        float targetTop = targetCorners[1].y;    // górna krawędź elementu
        float targetBottom = targetCorners[0].y; // dolna krawędź elementu
        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        Vector2 newPos = content.anchoredPosition;
        float scaleY = scrollRect.content.lossyScale.y;

        // 1️⃣ Element wyższy niż viewport → ustawiamy tak, by górna krawędź była wyrównana
        if ((targetTop - targetBottom) > (viewportTop - viewportBottom))
        {
            float delta = viewportTop - targetTop;
            newPos.y -= delta / scaleY;
        }
        else
        {
            // 2️⃣ Jeśli górna krawędź wystaje nad viewport → przewiń w dół
            if (targetTop > viewportTop)
            {
                float delta = targetTop - viewportTop;
                newPos.y -= delta / scaleY;
            }
            // 3️⃣ Jeśli dolna krawędź wystaje pod viewport → przewiń w górę
            else if (targetBottom < viewportBottom)
            {
                float delta = viewportBottom - targetBottom;
                newPos.y += delta / scaleY;
            }
        }

        // Animowane przesunięcie (DOTween)
        content.DOAnchorPos(newPos, scrollDuration).SetEase(Ease.OutCubic).SetUpdate(true);
    }
}
