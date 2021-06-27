using UnityEngine;

//Manager class to handle user UI events
public class UIEventHandler : MonoBehaviour
{
    //Reference to the queued monster spawner
    [SerializeField]
    QueuedMonsterSpawner spawner = null;

    //Create a new spawn object message and enqueue it into the message handler queue, called by spawn buttons when OnClick event is triggered by user
    public void SpawnObject(string objectToSpawn)
    {
        QueuedMonsterSpawner.SpawnerMessage message = new QueuedMonsterSpawner.SpawnerMessage(QueuedMonsterSpawner.SpawnerMessage.Message.spawnObject, objectToSpawn);
        spawner.Enqueue(message);
    }

    //Create a new clear queue message and enqueue it into the message handler queue, called by clear queue button when OnClick event is triggered by user
    public void ClearQueue()
    {
        QueuedMonsterSpawner.SpawnerMessage message = new QueuedMonsterSpawner.SpawnerMessage(QueuedMonsterSpawner.SpawnerMessage.Message.clearQueue, "Clear");
        spawner.PriorityEnqueue(message);
    }
}
