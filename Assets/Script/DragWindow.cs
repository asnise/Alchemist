using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _rectClampbronder;

    [SerializeField] private Canvas canvas;

    private Vector2 _minPosition;
    private Vector2 _maxPosition;

    private void Start()
    {
        UpdateBounds();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        ClampToWindowBounds();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.SetAsLastSibling();
    }

    private void ClampToWindowBounds()
    {
        if (_rectClampbronder == null)
        {
            Vector2 clampedPosition = _rectTransform.anchoredPosition;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, _minPosition.x, _maxPosition.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, _minPosition.y, _maxPosition.y);
            _rectTransform.anchoredPosition = clampedPosition;
        }
        else
        {
            Vector2 clampedPosition = _rectClampbronder.anchoredPosition;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, _minPosition.x, _maxPosition.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, _minPosition.y, _maxPosition.y);
            _rectTransform.anchoredPosition = clampedPosition;
        }
    }

    private void UpdateBounds()
    {
        Rect parentRect = _rectTransform.parent.GetComponent<RectTransform>().rect;
        Rect windowRect = _rectTransform.rect;

        _minPosition = new Vector2(
            -parentRect.width / 2 + windowRect.width / 2,
            -parentRect.height / 2 + windowRect.height / 2
        );

        _maxPosition = new Vector2(
            parentRect.width / 2 - windowRect.width / 2,
            parentRect.height / 2 - windowRect.height / 2
        );
    }
}
