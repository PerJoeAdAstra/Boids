using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    //Organise this

    //Change to get from global settings
    public string boidTag = "Boid";
    public bool randomiseColor = true;

    [Header("Movement Settings")]
    public float maxSpeed = 2f;
    public float minSpeed = 5f;
    public float turnSpeed = 5f;

    [Header("Movement Weights")]
    public float avoidBoidsWeight = 10f;
    public float avoidObstaclesWeight = 10f;
    public float similarDirectionWeight = 1f;
    public float boidCentreWeight = 1f;

    [Header("Vision Settings")]
    public float visionConeAngle = 260f;
    public float radius = 0.2f;
    public float collisionAvoidDistance = 1f;
    public float similarDirectionDistance = 1f;
    public float maxAvoidObstacleDistance = 0.5f;
    public LayerMask obstacleLayerMask;

    [Header("Debugging options")]
    public bool selected = false;
    public bool move = true;
    public bool turn = true;


    //Current State
    private Vector3 velocity;
    private Vector3 acceleration;

    //To update
    private Vector3 avoidBoidsVector;
    private Vector3 similarDirectionVector;
    private Vector3 boidCentreVector;
    private Vector3 avoidObstaclesVector;
    

    private MeshRenderer meshRenderer;
    private Color startColour;

    private float boidCentreAdjustableWeight;
    private float avoidBoidsAdjustableWeight;
    private float adjustableSpeed;
    private float adjustableTurnSpeed;
    private float maxInteractDistance;


    private List<Transform> nearbyBoids;

    public void Initialize()
    {
        float startSpeed = (minSpeed + maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }


    void Awake()
    {
        boidCentreAdjustableWeight = boidCentreWeight;
        avoidBoidsAdjustableWeight = avoidBoidsWeight;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        startColour = meshRenderer.material.color;

        if (randomiseColor)
            startColour = startColour + new Color(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        meshRenderer.material.color = startColour;

        maxInteractDistance = Mathf.Max(collisionAvoidDistance, similarDirectionDistance);
        Initialize();
    }


    public void Highlight()
    {
        meshRenderer.material.color = Color.gray;
    }

    public void DeHighlight()
    {
        meshRenderer.material.color = startColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
            Highlight();
        else
            DeHighlight();

        Vector3 acceleration = Vector3.zero;

        /*
        if (target)
        {
            Vector3 targetDirection = target.position - this.transform.position;
            acceleration = SteerTowards(targetDirections) * targetWeight;
        }
        */

        nearbyBoids = GetNearbyBoids();

        // Rule #1: Separation - Steer away from nearby boids to avoid crashing into them
        avoidBoidsVector = SteerTowards(CalculateAvoidBoidVector() * avoidBoidsWeight);
        

        // Rule #2: Alignment - Steer in the same direction as the nearbyBoids
        similarDirectionVector = SteerTowards(CalculateSimilarDirectionVector() * similarDirectionWeight);

        // Rule #3: Cohesion - Head towards the centre of nearby boids
        boidCentreVector = SteerTowards(CalculateBoidCentreVector() * avoidBoidsAdjustableWeight);

        acceleration += avoidBoidsVector;
        acceleration += similarDirectionVector;
        acceleration += boidCentreVector;

        // Rule #1.1: Steer away from environment to avoid crashing into environment
        if (IsHeadingForEnvironmentCollision())
        {
            Vector3 avoidObstaclesVector = NonCollidingPath();
            Vector3 collisionAvoidVector = SteerTowards(avoidObstaclesVector) * avoidObstaclesWeight;
            acceleration += collisionAvoidVector;
        }
        
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        velocity = dir * speed;
        if(selected)
            Debug.DrawLine(transform.position, transform.position + velocity, Color.red);

        if(move)
            transform.position += velocity * Time.deltaTime;
        
        if(turn)
            transform.forward = dir;
    }

    private List<Transform> GetNearbyBoids()
    {
        GameObject[] allBoids = GameObject.FindGameObjectsWithTag(boidTag);

        List<Transform> detectedBoids = new List<Transform>();
        foreach (GameObject boid in allBoids)
        {
            if (boid == this.gameObject) continue; //Don't need to do this for self
            float distance = Vector3.Distance(this.transform.position, boid.transform.position);
            if (distance < maxInteractDistance) //Check boid is within distance
            {
                Vector3 directionToBoid = boid.transform.position - transform.position;
                if (Vector3.Angle(this.transform.forward, directionToBoid) < visionConeAngle / 2f) //check is within viewing angle
                {
                    detectedBoids.Add(boid.transform);
                }
            }
        }
        return detectedBoids;
    }

    private Vector3 SteerTowards(Vector3 dir)
    {
        Vector3 temp = dir.normalized * maxSpeed - velocity;
        return temp;
    }
    
    //Update vector to avoid other boids
    private Vector3 CalculateAvoidBoidVector()
    {
        Vector3 avoidVector = new Vector3(0f, 0f, 0f);
        foreach (Transform boid in nearbyBoids)
        {
            float distance = Vector3.Distance(boid.position, this.transform.position);

            if (distance > collisionAvoidDistance)
                continue;

            Vector3 directionToBoid = boid.position - this.transform.position;
            
            if (selected)
                Debug.DrawLine(this.transform.position, boid.transform.position, Color.red);
            avoidVector -= directionToBoid.normalized * (collisionAvoidDistance - distance);
        }
        return avoidVector;
    }

    //Update vector to find common direction with neighbours
    private Vector3 CalculateSimilarDirectionVector()
    {
        Vector3 simDirVector = new Vector3(0f, 0f, 0f);
        foreach (Transform boid in nearbyBoids)
        {
            if (selected)
                Debug.DrawLine(this.transform.position, boid.transform.position, Color.green);
            simDirVector += boid.transform.forward;
        }
        return simDirVector;
    }

    private Vector3 CalculateBoidCentreVector()
    {
        Vector3 centreVector = Vector3.zero;
        foreach (Transform boid in nearbyBoids)
        {
            centreVector += boid.transform.position;
        }

        if (nearbyBoids.Count == 0)
        {
            boidCentreAdjustableWeight = 0f;
            return Vector3.zero;
        }

        boidCentreAdjustableWeight = boidCentreWeight;

        centreVector /= (float)nearbyBoids.Count;

        if (selected)
            Debug.DrawLine(this.transform.position, centreVector, Color.blue);

        return centreVector - this.transform.position;
    }
    
    private bool IsHeadingForEnvironmentCollision()
    {
        //Raycast forwards, if it is blocked we need to do something
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        return (Physics.SphereCast(ray, radius, maxAvoidObstacleDistance, obstacleLayerMask));
    }

    private Vector3 NonCollidingPath()
    {
        //Search for nearest avaliable safest direction.
        //*Also* want it to be nearest to the centre
        for (int i = 0; i < BoidHelper.directions.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(BoidHelper.directions[i]);
            //Debug.Log(dir);
            Ray ray = new Ray(this.transform.position, dir);
            if(selected)
                Debug.DrawLine(this.transform.position, ray.direction * 100f, Color.cyan);
            if (!Physics.SphereCast(ray, radius, maxAvoidObstacleDistance, obstacleLayerMask))
            {
                if (selected)
                    Debug.DrawLine(this.transform.position + 0.1f * BoidHelper.directions[i], this.transform.position + ray.direction, Color.magenta);
                return ray.direction;
            }
        }
        return this.transform.forward;
    }
    
}
