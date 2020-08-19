using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner2D : MonoBehaviour
{
    public int number;
    public Transform prefab;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        for (int i = 0; i < number; i++)
        {
            float x = Random.Range(0f, Screen.width);
            float y = Random.Range(0f, Screen.height);
            float theta = Random.Range(0f, 360f);

            Vector3 position = mainCamera.ScreenToWorldPoint(new Vector3(x, y, 10f));

            Quaternion rotation = Quaternion.Euler(0f, 0f, theta);

            Instantiate(prefab, position, rotation);
        }
    }
}
