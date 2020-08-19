using UnityEngine;

public class ScreenWraparound : MonoBehaviour
{
    [HideInInspector]
    public Camera mainCamera;

    private float xWorldMax;
    private float xWorldMin;
    private float yWorldMax;
    private float yWorldMin;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        Vector3 screenMax = new Vector3(Screen.width, Screen.height, 0f);
        Vector3 screenMin = new Vector3(0f, 0f, 0f);

        screenMax = mainCamera.ScreenToWorldPoint(screenMax);
        screenMin = mainCamera.ScreenToWorldPoint(screenMin);

        xWorldMax = screenMax.x;
        xWorldMin = screenMin.x;
        yWorldMax = screenMax.y;
        yWorldMin = screenMin.y;
    }

    private void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position); 

        if(screenPoint.x > Screen.width)
        {
            transform.position = new Vector3(xWorldMin, transform.position.y, transform.position.z);
        }
        else if (screenPoint.x < 0)
        {
            transform.position = new Vector3(xWorldMax, transform.position.y, transform.position.z);
        }

        if (screenPoint.y > Screen.height)
        {
            transform.position = new Vector3(transform.position.x, yWorldMin, transform.position.z);
        }
        else if(screenPoint.y < 0)
        {
            transform.position = new Vector3(transform.position.x, yWorldMax, transform.position.z);
        }
    }
}
