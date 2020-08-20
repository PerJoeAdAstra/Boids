using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePlotter : MonoBehaviour
{
    public Transform prefab;

    public int numberOfDirections;
    public float viewAngle;

    // Start is called before the first frame update
    void Start()
    {
        PlotSphere();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlotSpiral()
    {
        List<Vector2> directions = new List<Vector2>();
        for (int i = 0; i < numberOfDirections; i++)
        {
            float temp = i + 0.5f;
            float r = Mathf.Pow((temp / numberOfDirections), 0.5f);
            float theta = Mathf.PI * (1 + Mathf.Pow(5, 0.5f)) * temp;
            Vector2 direction = new Vector2(r * Mathf.Cos(theta), r*Mathf.Sin(theta));
            directions.Add(direction);
        }

        foreach(Vector2 direction in directions)
        {
            Instantiate(prefab, direction, Quaternion.identity);
        }
    }

    private void PlotSphere()
    {
        List<Vector3> directions = new List<Vector3>();
        for (int i = 0; i < numberOfDirections; i++)
        {
            float temp = i + 0.5f;
            float phi = Mathf.Acos(1f - 2f * temp / numberOfDirections);
            if (Mathf.Rad2Deg * phi < viewAngle) continue;
            float theta = Mathf.PI * (1 + Mathf.Pow(5, 0.5f)) * temp;
                                            //cos(theta)     * sin(phi),             sin(theta) * sin(phi),       cos(phi);
            Vector3 direction = new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(phi));
            directions.Add(direction);
        }

        foreach (Vector3 direction in directions)
        {
            GameObject obj = Instantiate(prefab, direction, Quaternion.identity).gameObject;
        }
    }
}
