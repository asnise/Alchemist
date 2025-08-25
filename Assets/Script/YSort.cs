using UnityEngine;

public class YSort : MonoBehaviour
{
    public float _yOffset = 0f;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = transform.GetSortingOrder(_yOffset);
    }

    private void OnDrawGizmos()
    {


        Vector3 referencePoint = transform.position + new Vector3(0, _yOffset, 0);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(referencePoint, 0.1f);
    }
}