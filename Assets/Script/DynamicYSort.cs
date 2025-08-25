using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicYSort : MonoBehaviour
{
    private int _baseSortingOrder;
    private float _YSortOffset;
    [SerializeField]
    private SortableSprite[] _sprites;
    [SerializeField]
    private Transform _sortOffsetMaker;

    private void Start()
    {
        _YSortOffset = _sortOffsetMaker.position.y;
    }

    private void Update()
    {
        _baseSortingOrder = transform.GetSortingOrder(_YSortOffset);

        foreach (var sortableSprite in _sprites)
        {
            if (sortableSprite.spriteRenderer != null)
            {
                sortableSprite.spriteRenderer.sortingOrder = _baseSortingOrder + sortableSprite.retativeOrder;
            }
        }
    }

    [Serializable]
    public struct SortableSprite
    {
        public SpriteRenderer spriteRenderer;
        public int retativeOrder;
    }
}



