using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public float zoomSpeed = 4f;
    public float minSize = 5f; // Minimum orthographic size
    public float maxSize = 15f; // Maximum orthographic size
    private Camera cam;

    public bool limitFollow = true;
    public Vector2 minXAndY;
    public Vector2 maxXAndY;

    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam.orthographic)
        {
            cam.orthographicSize = minSize; // Set initial size
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            if (limitFollow)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minXAndY.x, maxXAndY.x);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minXAndY.y, maxXAndY.y);
            }

            if (isShaking)
            {
                smoothedPosition += Random.insideUnitSphere * shakeMagnitude;
            }

            transform.position = smoothedPosition;
        }
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f && cam.orthographic)
        {
            cam.orthographicSize -= scrollInput * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);
        }
    }

    // Camera Shake Function
    public void ShakeCamera(float duration, float magnitude)
    {
        if (!isShaking)
        {
            StartCoroutine(Shake(duration, magnitude));
        }
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        isShaking = true;
        shakeDuration = duration;
        shakeMagnitude = magnitude;

        while (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;
            yield return null;
        }

        isShaking = false;
    }
}
