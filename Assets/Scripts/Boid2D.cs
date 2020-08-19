using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid2D : MonoBehaviour
{
    //Organise this

    //Change to get from global settings
    public float speed = 2f;
    public float turnSpeed = 2f;
    public int visionSamples = 200;
    public float visionConeAngle = 2.5f;
    public float collisionAvoidDistance = 0.2f;
    public float similarDirectionDistance = 1f;
    public float maxSlowAngle = 90f;

    public string boidTag = "Boid";
    private Vector3 forward;

    public bool randomiseColor = true;
    public bool selected = false;

    public float avoidBoidsWeight = 10f;
    public float avoidObstaclesWeight = 10f;
    public float similarDirectionWeight = 1f;
    public float boidCentreWeight = 1f;
    //More Here

    private Vector3 avoidBoidsVector = new Vector3(0f, 0f, 0f);
    private Vector3 similarDirectionVector = new Vector3(0f, 0f, 0f);
    private Vector3 boidCentreVector = new Vector3(0f, 0f, 0f);
    private Vector3 avoidObstaclesVector = new Vector3(0f, 0f, 0f);

    private SpriteRenderer spriteRenderer;
    private MeshRenderer meshRenderer;
    private Color startColour;

    private float boidCentreAdjustableWeight;
    private float adjustableSpeed;

    void Awake()
    {
        boidCentreAdjustableWeight = boidCentreWeight;
        adjustableSpeed = speed;

        forward = new Vector3(1, 0, 0);

        

        if(!TryGetComponent<SpriteRenderer>(out spriteRenderer))
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (spriteRenderer)
            startColour = spriteRenderer.color;
        else
            startColour = meshRenderer.material.color;

        if (randomiseColor)
            startColour = startColour + new Color(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        if (spriteRenderer)
            spriteRenderer.color = startColour;
        else
            meshRenderer.material.color = startColour;
    }

    public void Highlight()
    {
        if (spriteRenderer) spriteRenderer.color = Color.gray;
        else meshRenderer.material.color = Color.gray;
    }

    public void DeHighlight()
    {
        if (spriteRenderer) spriteRenderer.color = startColour;
        else meshRenderer.material.color = startColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
            Highlight();
        else
            DeHighlight();

        Move();
    }

    private void Move()
    {

        // Rule #1: Separation - Steer away from nearby boids to avoid crashing into them
        UpdateAvoidBoidVector();

        // Rule #1.1: Steer away from environment to avoid crashing into environment
        //UpdateAvoidEnvironmentVector();

        // Rule #2: Alignment - Steer in the same direction as the nearbyBoids
        UpdateSimiarDirectionVector();

        // Rule #3: Cohesion - Head towards the centre of nearby boids
        UpdateBoidCentreVector();

        //If you need to turn you should slow down

        Vector3 directionVector = transform.up + (avoidBoidsVector * avoidBoidsWeight) + (similarDirectionWeight * similarDirectionVector) + (boidCentreAdjustableWeight * boidCentreVector);
        directionVector.Normalize();

        AdjustSpeed(directionVector);
        if(selected)
            Debug.DrawLine(transform.position, transform.position + ((directionVector)*adjustableSpeed/speed));
        SteerTowards(directionVector);

        MoveForwards();
    }

    private void AdjustSpeed(Vector3 dir)
    {

        float angle = Mathf.Clamp(Mathf.Abs(Vector3.Angle(transform.up, dir)), 0f, maxSlowAngle);
        if(selected)
            //Debug.Log("Angle:" + angle + ", " + Vector3.Angle(transform.up, dir));
        adjustableSpeed = (3* speed / 4f) + (speed / 4f) * (maxSlowAngle - angle)/maxSlowAngle;
    }

    private void SteerTowards(Vector3 dir)
    {
        Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.back);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);
    }

    private void MoveForwards()
    {
        transform.Translate(transform.up * adjustableSpeed * Time.deltaTime, Space.World);
    }

    private void LookInDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    //Update vector to avoid other boids
    private void UpdateAvoidBoidVector()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag(boidTag);

        Vector3 avoidVector = new Vector3(0f, 0f, 0f);
        foreach (GameObject boid in boids)
        {
            if (boid == this.gameObject) continue; //Don't need to do this for self
            float distance = Vector3.Distance(this.transform.position, boid.transform.position);
            if (distance < collisionAvoidDistance) //Check boid is within distance
            {
                Vector3 directionToBoid = boid.transform.position - transform.position;
                if (Vector3.Angle(this.transform.up, directionToBoid) < visionConeAngle/2f) //check is within viewing angle
                {
                    if (selected)
                    {
                        Debug.DrawLine(this.transform.position, boid.transform.position, Color.red * (collisionAvoidDistance - distance) / collisionAvoidDistance);
                        
                    }
                    avoidVector -= directionToBoid * (collisionAvoidDistance - distance);
                }
            }
        }
        avoidBoidsVector = avoidVector.normalized;
    }

    //Update vector to find common direction with neighbours
    private void UpdateSimiarDirectionVector()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag(boidTag);

        Vector3 simDirVector = new Vector3(0f, 0f, 0f);
        foreach (GameObject boid in boids)
        {
            if (boid == this.gameObject) continue; //Don't need to do this for self
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < similarDirectionDistance) //Check boid is within distance
            {
                Vector3 directionToBoid = transform.position - boid.transform.position;
                if (Vector3.Angle(this.transform.up, directionToBoid) < visionConeAngle) //check is within viewing angle
                {
                    if (selected)
                        Debug.DrawLine(this.transform.position, boid.transform.position, Color.green);
                    simDirVector += boid.transform.up;
                }
            }
        }
        similarDirectionVector = simDirVector.normalized;
    }

    private void UpdateBoidCentreVector()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag(boidTag);

        Vector3 centreVector = new Vector3(0f, 0f, 0f);
        int boidCount = 0;
        foreach (GameObject boid in boids)
        {
            if (boid == this.gameObject) continue; //Don't need to do this for self
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < similarDirectionDistance) //Check boid is within distance
            {
                Vector3 directionToBoid = transform.position - boid.transform.position;
                if (Vector3.Angle(this.transform.up, directionToBoid) < visionConeAngle) //check is within viewing angle
                {
                    centreVector += boid.transform.position;
                    boidCount++;
                }
            }
        }

        if (boidCount == 0)
        {
            boidCentreAdjustableWeight = 0f;
            return;
        }

        boidCentreAdjustableWeight = boidCentreWeight;

        centreVector /= (float)boidCount;

        if (selected)
            Debug.DrawLine(this.transform.position, centreVector, Color.blue);

        boidCentreVector = centreVector - this.transform.position;
    }

    /*
    //Gets list of other boids that the current boid can see
    private List<Transform> GetVisibleBoids()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag(boidTag);

        List<Transform> boidsInRange = new List<Transform>();
        foreach (GameObject boid in boids)
        {
            if (boid == this) continue; //Don't need to do this for self
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < collisionAvoidDistance) //Check boid is within distance
            {
                Vector3 directionToBoid = transform.position - boid.transform.position;
                if(Vector3.Angle(forward, directionToBoid) < visionConeAngle) //check is within viewing angle
                {
                    boidsInRange.Add(boid.transform);
                }
            }
        }
        return boidsInRange;
    }
    */


    //LOOP OVER ALL OTHER BOIDS TO SEE STUFF!
    /*
    private bool IsHeadingForCollision()
    {
        //Raycast forwards, if it is blocked we need to do something
        RaycastHit hit;
        Debug.DrawRay(transform.position, forward, Color.blue);
        if (!Physics.SphereCast(transform.position, radius, forward, out hit))
        {
            return true;
        }
        return false;
    }

    private Vector3 AvoidCollisions()
    {
        //Search for nearest avaliable safest direction.
        //*Also* want it to be nearest to the centre
        //Directions don't change, store them in another class?

        float angle = -visionConeAngle / 2;
        float angleStep = visionConeAngle / (float) visionSamples;
        for (int i = 0; i < visionSamples; i ++)
        {
            Vector3 rayDirection = Quaternion.Euler(-visionConeAngle/2 + i * angleStep, 0f, 0f) * forward;
            RaycastHit hit;
            if (!Physics.SphereCast(transform.position, radius, rayDirection, out hit))
            {
                Debug.DrawRay(transform.position, rayDirection, Color.red, 1f);
                return rayDirection;
            }
        }
        return forward;
    }
    */
}
