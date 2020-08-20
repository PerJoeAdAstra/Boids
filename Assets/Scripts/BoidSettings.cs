using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float minSpeed = 2f;
    public float turnSpeed = 3f;

    [Header("Movement Weights")]
    public float avoidBoidsWeight = 5f;
    public float avoidObstaclesWeight = 100f;
    public float similarDirectionWeight = 1f;
    public float boidCentreWeight = 1f;
    public float targetWeight = 1f;

    [Header("Vision Settings")]
    public float visionConeAngle = 260f;
    public float sphereCastRadius = 0.25f;
    public float avoidBoidsDistance = 1f;
    public float similarDirectionDistance = 2.5f;
    public float avoidObstaclesDistance = 1f;
    public LayerMask obstacleLayerMask;
}
