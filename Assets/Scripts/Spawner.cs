using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
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
            //float x = Random.Range(-5f, 5f);
            //float y = Random.Range(-5f, 5f);
            //float z = Random.Range(-5f, 5f);
            float theta = Random.Range(0f, 360f);
            float theta1 = Random.Range(0f, 360f);

            Vector3 position = new Vector3(0, 0, 0);

            Quaternion rotation = Quaternion.Euler(theta, theta1, 0);

            Instantiate(prefab, position, rotation);
        }
    }
}
