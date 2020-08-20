using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int number;
    public GameObject prefab;
    public float size = 5f;

    public BoidSettings settings;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < number; i++)
        {
            float theta = Random.Range(0f, 360f);
            float theta1 = Random.Range(0f, 360f);

            Vector3 position = new Vector3(0, 0, 0);

            Quaternion rotation = Quaternion.Euler(theta, theta1, 0);

            GameObject boid = Instantiate(prefab, position, rotation);
            boid.GetComponent<Boid>().Initialize(settings, target);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(size*2, size*2, size*2));
    }
}
