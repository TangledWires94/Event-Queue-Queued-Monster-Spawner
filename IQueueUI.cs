using UnityEngine;

//Interface to ensure that no matter which version of the queue UI the scene is using the QueuedMonsterSpawner doesn't need to change
public interface IQueueUI
{
    public int MaxQueueSize { get; } //Property to allow QueuedMonsterSpawner to check the maximum size of the UI queue
    public void Enqueue(Texture newElement); //Enqueue new message image at the end of the queue UI
    public void PriorityEnqueue(Texture newElement); //Enqueue new message image at the start of the queue UI
    public void Dequeue(); //Dequeue element from the start of the queue UI and shift other elements down
    public void ClearQueue(); //Reinitialise queue back to empty
}
