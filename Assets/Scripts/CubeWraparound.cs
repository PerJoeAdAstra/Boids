using UnityEngine;

public class CubeWraparound : MonoBehaviour
{
    [HideInInspector]
    public Camera mainCamera;

    public Vector3 cubeCentre = new Vector3(0f, 0f, 0f);
    public float cubeSize = 10f;


    private float xWorldMax;
    private float xWorldMin;
    private float yWorldMax;
    private float yWorldMin;
    private float zWorldMax;
    private float zWorldMin;

    private void Start()
    {
        xWorldMax = cubeCentre.x + cubeSize/2f;
        xWorldMin = cubeCentre.x - cubeSize / 2f;
        yWorldMax = cubeCentre.y + cubeSize / 2f;
        yWorldMin = cubeCentre.y - cubeSize / 2f;
        zWorldMax = cubeCentre.z + cubeSize / 2f;
        zWorldMin = cubeCentre.z - cubeSize / 2f;

    }

    private void Update()
    {
        Vector3 pos = transform.position; 

        if(pos.x > xWorldMax)
        {
            transform.position = new Vector3(xWorldMin, transform.position.y, transform.position.z);
        }
        else if (pos.x < xWorldMin)
        {
            transform.position = new Vector3(xWorldMax, transform.position.y, transform.position.z);
        }

        if (pos.y > yWorldMax)
        {
            transform.position = new Vector3(transform.position.x, yWorldMin, transform.position.z);
        }
        else if(pos.y < yWorldMin)
        {
            transform.position = new Vector3(transform.position.x, yWorldMax, transform.position.z);
        }

        if (pos.z > zWorldMax)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zWorldMin);
        }
        else if (pos.z < zWorldMin)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zWorldMax);
        }
    }
}
