using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that implements the IQueueUI interface with no implementation in the functions to allow monsterspawner to work with no queue UI present in the scene
public class NullQueueUI : MonoBehaviour, IQueueUI
{
    //Set big max queue size because without the restarint of the number of queue elements the monster spawner message queue can be as big as it wants
    public int MaxQueueSize { get { return 10000;} } 

    public void ClearQueue()
    {

    }

    public void Dequeue()
    {

    }

    public void Enqueue(Texture newElement)
    {
        
    }

    public void PriorityEnqueue(Texture newElement)
    {
        
    }
}
