using UnityEngine;

//Code to control movement of the moving units across the scene
public class UnitMovement : MonoBehaviour
{
    //Rate at which unit speed can increase both on land and in the air
    [SerializeField, Min(0f)]
    float speed = 1f, groundAcceleration = 10f, airAcceleration = 1f;

    //Gravity is applied in the velocity calculation rather than through Unity's physics engine
    [SerializeField]
    float gravity = -9.8f;

    [SerializeField]
    Vector3 direction = Vector3.right;
    Vector3 velocity = Vector3.zero;

    //Used to determine what acceleration to use and trigger animation changes
    bool inAir = true, landed = false;

    //References to the units rigidbody, animator and respawn trigger components
    Rigidbody rb;
    Animator anim;
    TriggerRespawn respawnTrigger = null;

    //Get references to components and initilaise the velocity field
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        velocity = rb.velocity;
        anim = GetComponent<Animator>();
        respawnTrigger = GetComponent<TriggerRespawn>();
    }

    //Move unit towards the right side of the screen at the configured speed by updating its velocity every frame, using fixed update to keep movement smooth between frames
    private void FixedUpdate()
    {
        //Get current velocity
        velocity = rb.velocity;

        //Set new target velocities
        float targetX = direction.x * speed;
        float targetY = gravity;
        float targetZ = direction.z * speed;

        //if the unit is in the air use the air acceleration, otherwise use standard ground acceleration
        float acceleration = inAir ? airAcceleration : groundAcceleration;

        //Move from the current velocity towards the target velocity by the amount allowed by the acceleration value, no acceleration on the Y component, velocity = gravity
        float newX = Mathf.MoveTowards(velocity.x, targetX, acceleration);
        float newY = targetY;
        float newZ = Mathf.MoveTowards(velocity.z, targetZ, acceleration);
        velocity = new Vector3(newX, targetY, newZ);

        //Update animation speed to match the new movement speed in the x direction, makes movement look more realistic
        anim.speed = newX / speed;

        //Update unit velocity with the new calculated velocity
        rb.velocity = velocity;
    }

    //When unit collides with the ground update the animator and boolean values
    private void OnCollisionStay(Collision collision)
    {
        if (!landed)
        {
            landed = true;
            anim.SetTrigger("Landed");
        }
        inAir = false;
        anim.SetBool("inAir", false);
    }

    //When the unit leaves the ground update the inAir boolean and animator
    private void OnCollisionExit(Collision collision)
    {
        inAir = true;
        anim.SetBool("inAir", true);
    }

    //Handles collision with different triggers, runs different code depending on the trigger tag
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //Destroy the game object
            case "DestroyTrigger":
                Destroy(gameObject);
                break;

            default:
                break;
        }
    }

    //Handles unit leaving a trigger, runs different code depending on the trigger tag
    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            //Trigger respawn event to allow spawner to continue spawning
            case "RespawnTrigger":
                respawnTrigger.triggerRespawn.Invoke();
                break;
            default:
                break;
        }
    }
}
