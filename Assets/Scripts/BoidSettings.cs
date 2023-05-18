using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float maxSteerForce = 3f;

    [Header("Vision Settings")]
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1f; 

    [Header("Movement Weights")]
    public float alignWeight = 1f;
    public float cohesionWeight = 1f;
    public float seperateWeight = 1f;
    public float targetWeight = 1f;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = 0.27f;
    public float avoidCollisionsWeight = 10f;
    public float collisionAvoidDst = 5f;
    
}
