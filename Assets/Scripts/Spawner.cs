using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int number;
    public Transform prefab;
    public float size = 5f;

    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = Color.white;
        color.a = 0.2f;

        for (int i = 0; i < number; i++)
        {
            float theta = Random.Range(0f, 360f);
            float theta1 = Random.Range(0f, 360f);

            Vector3 position = new Vector3(0, 0, 0);

            Quaternion rotation = Quaternion.Euler(theta, theta1, 0);

            Instantiate(prefab, position, rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, new Vector3(size*2, size*2, size*2));
    }
}
