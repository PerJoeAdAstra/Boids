using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbit : MonoBehaviour
{
    public Transform objectToOrbit;
    public float orbitSpeed = 1.0f;
    public float orbitRadius = 5.0f;
    private float currentAngle = 0.0f;
    public float offset = 0.0f;

    private Vector3 orbitCentre;


    private void Awake()
    {
        orbitCentre = objectToOrbit.position; //localposition?
        currentAngle = offset;
    }

    private void Update()
    {
        float x = orbitCentre.x + orbitRadius * Mathf.Cos(currentAngle);
        float y = orbitCentre.y + orbitRadius * Mathf.Sin(currentAngle);

        Vector3 newPos = new Vector3(x, 0f, y);

        this.GetComponent<Transform>().localPosition = newPos;

        currentAngle += orbitSpeed;
        currentAngle = currentAngle % (2.0f * Mathf.PI);
    }
}
