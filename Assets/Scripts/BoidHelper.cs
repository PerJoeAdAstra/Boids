using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidHelper
{
    const int numberOfDirections = 300;
    const float viewAngle = 210f;
    public static readonly Vector3[] directions;

    static BoidHelper()
    {
        directions = new Vector3[BoidHelper.numberOfDirections];
    
        for (int i = 0; i < numberOfDirections; i++)
        {
            float temp = i + 0.5f;
            float phi = Mathf.Acos(1f - 2f * temp / numberOfDirections);
            if (Mathf.Rad2Deg * phi > viewAngle) continue;

            float theta = Mathf.PI * (1 + Mathf.Pow(5, 0.5f)) * temp;
            directions[i] = new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(phi)).normalized;
        }
    }
}
