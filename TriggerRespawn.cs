using UnityEngine;

//Component which holds the Trigegr Respawn event, used to define which objects can be spawned by the monster spawner because objects must be able to trigger a respawn
public class TriggerRespawn : MonoBehaviour
{
    public delegate void SpawnedObjectEvent();
    public SpawnedObjectEvent triggerRespawn;

    //Whever a game object with this component is destroyed clear the subscription lists so that referemces are cleared correctly
    private void OnDestroy()
    {
        triggerRespawn = null;
    }
}
