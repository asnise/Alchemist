using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasColorRandom : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject.");
            return;
        }
        // Generate a random color
        Color randomColor = new Color(Random.value, Random.value, Random.value, 1.0f);
        // Apply the random color to the sprite
        spriteRenderer.color = randomColor;
    }
}
