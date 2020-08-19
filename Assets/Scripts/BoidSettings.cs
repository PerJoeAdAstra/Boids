using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    //settings
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float visionCodeAngle = 2.5f;
    public float avoidanceRadius = 1f;
    //public float maxSteerForce = 3f;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = 0.27f;
    public float avoidCollisionsWeight = 10f;
    public float collisionsAvoidDst = 5f;
}
