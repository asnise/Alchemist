using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestoryDelay : MonoBehaviour
{
    public float cooldown = 3.0f; // Cooldown time in seconds
    public UnityEvent EventAfter;

    void Start()
    {
        StartCoroutine(DestroyAfterCooldown());
    }

    IEnumerator DestroyAfterCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        if(EventAfter != null)
        {
            EventAfter.Invoke();
        }
        Destroy(gameObject);
    }
}
