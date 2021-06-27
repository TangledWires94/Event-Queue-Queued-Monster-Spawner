using UnityEngine;

//Triggers an animation and TriggerRespawn event after a set time delay
[RequireComponent(typeof(Animator), typeof(TriggerRespawn))]
public class TimedBomb : MonoBehaviour
{
    //Time delay in seconds between spawning and detonating
    [SerializeField, Min(0f)]
    float detonationTime = 1f;

    //Time at which the object was spawned
    float spawnTime = 0f;

    //Reference to animator component
    Animator anim = default;

    //Event to trigger the queued message handler to allow spawning of new objects
    TriggerRespawn respawnTrigger = null;

    //Get reference to the animator and TriggerRespawn component and get spawn time
    private void Awake()
    {
        anim = GetComponent<Animator>();
        respawnTrigger = GetComponent<TriggerRespawn>();
        spawnTime = Time.time;
    }

    //Check if detonation time has elapsed, if so start detonation
    private void Update()
    {
        if(Time.time > spawnTime + detonationTime)
        {
            anim.SetTrigger("Detonate");
        }
    }

    //Trigger respawn event, clear subscription to it and then destroy this game object
    public void Detonate()
    {
        respawnTrigger.triggerRespawn.Invoke();
        Destroy(gameObject);
    }
}
