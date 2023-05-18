using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    //Change to get from global settings
    public string boidTag = "Boid";
    public bool randomiseColor = true;

    private BoidSettings settings;

    [Header("Debugging options")]
    public bool selected = false;
    public bool move = true;
    public bool turn = true;

    [HideInInspector]
    public Vector3 forward;
    [HideInInspector]
    public Vector3 position;
    private Vector3 velocity;

    //To update:
    private Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPercievedFlockmates;

    //Cached
    private Material material;
    private Color startColour;
    private Transform cachedTransform;
    private Transform target;

    private void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        startColour = material.color;

        if (randomiseColor)
            startColour = startColour + new Color(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        material.color = startColour;

        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings)
    {
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        if (material != null)
        {
            material.color = col;
        }
    }

    public void SetTarget(Transform transform)
    {
        RemoveTarget();
        target = transform;
    }

    public void RemoveTarget()
    {
        target = null;
    }

    public void Highlight()
    {
        material.color = Color.gray;
    }

    public void DeHighlight()
    {
        material.color = startColour;
    }

    public void UpdateBoid()
    {
        if (selected)
            Highlight();
        else
            DeHighlight();

        Vector3 acceleration = Vector3.zero;

        if (target != null)
        {
            Vector3 targetDirection = target.position - position;
            acceleration = SteerTowards(targetDirection) * settings.targetWeight;
        }

        if(numPercievedFlockmates != 0)
        {
            centreOfFlockmates /= numPercievedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            // Rule #2: Alignment - Steer in the same direction as the nearbyBoids
            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;

            // Rule #3: Cohesion - Head towards the centre of nearby boids
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;

            // Rule #1: Separation - Steer away from nearby boids to avoid crashing into them
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;

        }

        // Rule #1.1: Steer away from environment to avoid crashing into environment
        if (IsHeadingForEnvironmentCollision())
        {
            Vector3 avoidObstaclesDir= NonCollidingPath();
            Vector3 collisionAvoidForce = SteerTowards(avoidObstaclesDir) * settings.avoidCollisionsWeight;
            acceleration += collisionAvoidForce;
        }
        
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;

        position = cachedTransform.position;
        forward = dir;
    }

    private Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }

    private bool IsHeadingForEnvironmentCollision()
    {
        RaycastHit hit;
        return Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask);
    }

    private Vector3 NonCollidingPath()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < BoidHelper.directions.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }
        return forward;
    }
}
