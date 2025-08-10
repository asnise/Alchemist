using UnityEngine;
using UnityEngine.UI;

public class UIFollowTarget : MonoBehaviour
{
    public GameObject target;
    private Vector2 lastScreenPos;
    public Vector2 offset;
    void LateUpdate()
    {
        if (target != null)
        {
            lastScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        }

        transform.position = lastScreenPos + offset;
    }
}
