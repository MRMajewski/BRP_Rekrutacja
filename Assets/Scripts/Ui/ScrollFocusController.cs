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

        Vector3[] targetCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        target.GetWorldCorners(targetCorners);
        viewport.GetWorldCorners(viewportCorners);

        float targetTop = targetCorners[1].y;    
        float targetBottom = targetCorners[0].y; 
        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        Vector2 newPos = content.anchoredPosition;
        float scaleY = scrollRect.content.lossyScale.y;

        if ((targetTop - targetBottom) > (viewportTop - viewportBottom))
        {
            float delta = viewportTop - targetTop;
            newPos.y -= delta / scaleY;
        }
        else
        {

            if (targetTop > viewportTop)
            {
                float delta = targetTop - viewportTop;
                newPos.y -= delta / scaleY;
            }
            else if (targetBottom < viewportBottom)
            {
                float delta = viewportBottom - targetBottom;
                newPos.y += delta / scaleY;
            }
        }

        content.DOAnchorPos(newPos, scrollDuration).SetEase(Ease.OutCubic).SetUpdate(true);
    }
}
