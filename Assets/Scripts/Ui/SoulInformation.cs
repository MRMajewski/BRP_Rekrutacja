using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulInformation : MonoBehaviour, ISelectHandler
{
    [SerializeField] private Image MainImage;
    [SerializeField] private Button SoulButton;
    [HideInInspector] public SoulItem soulItem;
    [SerializeField] private ScrollFocusController scrollFocusController;
    public void SetSoulItem(SoulItem _soulItem, Action OnSoulClick = null)
    {
        soulItem = _soulItem;
        MainImage.sprite = soulItem.Avatar;
        if (OnSoulClick != null) SoulButton.onClick.AddListener(() => OnSoulClick());
    }

    public void OnSelect(BaseEventData eventData)
    {

        if (scrollFocusController != null)
        {
            scrollFocusController.EnsureSelectedVisible(this.GetComponent<RectTransform>());
        }

    }
}