using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    //Organise this

    //Change to get from global settings
    public string boidTag = "Boid";
    public bool randomiseColor = true;

    private BoidSettings settings;

    [Header("Debugging options")]
    public bool selected = false;
    public bool move = true;
    public bool turn = true;


    //Current State -> update similar direction to use velocity of other boids
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 acceleration;

    //To update
    private Vector3 avoidBoidsVector;
    private Vector3 similarDirectionVector;
    private Vector3 boidCentreVector;
    private Vector3 avoidObstaclesVector;
    
    //Astetics
    private MeshRenderer meshRenderer;
    private Color startColour;

    private float boidCentreAdjustableWeight;
    private float avoidBoidsAdjustableWeight;
    private float adjustableSpeed;
    private float adjustableTurnSpeed;
    private float maxInteractDistance;

    private Transform target;


    private List<Transform> nearbyBoids;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        startColour = meshRenderer.material.color;

        if (randomiseColor)
            startColour = startColour + new Color(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        meshRenderer.material.color = startColour;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.settings = settings;
        this.target = target;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;

        boidCentreAdjustableWeight = settings.boidCentreWeight;
        avoidBoidsAdjustableWeight = settings.avoidBoidsWeight;

        maxInteractDistance = Mathf.Max(settings.avoidBoidsDistance, settings.similarDirectionDistance);
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
        if (!settings) return;

        if (selected)
            Highlight();
        else
            DeHighlight();

        Vector3 acceleration = Vector3.zero;

        if (target)
        {
            Vector3 targetDirection = target.position - this.transform.position;
            acceleration = SteerTowards(targetDirection).normalized * settings.targetWeight;
        }
        

        nearbyBoids = GetNearbyBoids();

        // Rule #1: Separation - Steer away from nearby boids to avoid crashing into them
        avoidBoidsVector = SteerTowards(CalculateAvoidBoidVector().normalized * settings.avoidBoidsWeight);
        

        // Rule #2: Alignment - Steer in the same direction as the nearbyBoids
        similarDirectionVector = SteerTowards(CalculateSimilarDirectionVector().normalized * settings.similarDirectionWeight);

        // Rule #3: Cohesion - Head towards the centre of nearby boids
        boidCentreVector = SteerTowards(CalculateBoidCentreVector().normalized * settings.boidCentreWeight);

        acceleration += avoidBoidsVector;
        acceleration += similarDirectionVector;
        acceleration += boidCentreVector;

        // Rule #1.1: Steer away from environment to avoid crashing into environment
        if (IsHeadingForEnvironmentCollision())
        {
            Vector3 avoidObstaclesVector = NonCollidingPath();
            Vector3 collisionAvoidVector = SteerTowards(avoidObstaclesVector) * settings.avoidObstaclesWeight;
            acceleration += collisionAvoidVector;
        }
        
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
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
                if (Vector3.Angle(this.transform.forward, directionToBoid) < settings.visionConeAngle / 2f) //check is within viewing angle
                {
                    detectedBoids.Add(boid.transform);
                }
            }
        }
        return detectedBoids;
    }

    private Vector3 SteerTowards(Vector3 dir)
    {
        Vector3 temp = dir.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(temp, settings.turnSpeed);
    }
    
    //Update vector to avoid other boids
    private Vector3 CalculateAvoidBoidVector()
    {
        Vector3 avoidVector = new Vector3(0f, 0f, 0f);
        foreach (Transform boid in nearbyBoids)
        {
            float distance = Vector3.Distance(boid.position, this.transform.position);

            if (distance > settings.avoidBoidsDistance)
                continue;

            Vector3 directionToBoid = boid.position - this.transform.position;
            
            if (selected)
                Debug.DrawLine(this.transform.position, boid.transform.position, Color.red);
            avoidVector -= directionToBoid.normalized / 2f + (directionToBoid.normalized / (distance * 2f)) / 2f;
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

        boidCentreAdjustableWeight = settings.boidCentreWeight;

        centreVector /= (float)nearbyBoids.Count;

        if (selected)
            Debug.DrawLine(this.transform.position, centreVector, Color.blue);

        return centreVector - this.transform.position;
    }
    
    private bool IsHeadingForEnvironmentCollision()
    {
        //Raycast forwards, if it is blocked we need to do something
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        return (Physics.SphereCast(ray, settings.sphereCastRadius, settings.avoidObstaclesDistance, settings.obstacleLayerMask));
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
            if (!Physics.SphereCast(ray, settings.sphereCastRadius, settings.avoidObstaclesDistance, settings.obstacleLayerMask))
            {
                if (selected)
                    Debug.DrawLine(this.transform.position + 0.1f * BoidHelper.directions[i], this.transform.position + ray.direction, Color.magenta);
                return ray.direction;
            }
        }
        return this.transform.forward;
    }
    
}
