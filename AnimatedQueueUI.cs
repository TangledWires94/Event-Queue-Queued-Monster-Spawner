using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MyExceptions;

//WORK IN PROGRESS, Alternate implementation of the queue UI interface which triggers movement through an animator controller
public class AnimatedQueueUI : MonoBehaviour, IQueueUI
{
    //Individual elements of the queue UI
    [SerializeField]
    List<RawImage> queueElements = new List<RawImage>();

    Animator anim = null;
    int maxQueueSize = 0, queueIndex = 0;
    public int MaxQueueSize { get { return maxQueueSize; } }
    Texture nextTexture = null;

    //Set all raw image textures to null, get reference to animator component and set max queue size based on number of UI elements
    private void Awake()
    {
        ClearQueue();
        anim = GetComponent<Animator>();
        maxQueueSize = queueElements.Count;
    }

    //Activate "Enqueue" trigger on the animator to enqueue the next element into the end of the queue, only called by the monster spawner when a new message is added to 
    //the spawner queue
    public void Enqueue(Texture newElement)
    {
        if(queueIndex + 1 < maxQueueSize)
        {
            nextTexture = newElement;
            anim.SetTrigger("Enqueue");
            queueIndex++;
        }
        else
        {
            throw new QueueSizeException();
        }
    }

    //Alternate enqueue for when message is enqueued to the front of the queue rather than the end
    public void PriorityEnqueue(Texture newElement)
    {
        if (queueIndex + 1 < maxQueueSize)
        {
            for (int i = queueIndex; i > 0; i++)
            {
                queueElements[i].texture = queueElements[i - 1].texture;
            }
            queueElements[0].texture = newElement;
            anim.SetTrigger("Enqueue");
            queueIndex++;
        } 
        else
        {
            throw new QueueSizeException();
        }
    }

    //Activate "Dequeue" trigger on the animator to enqueue the next element into the end of the queue, only called by the monster spawner when a new message is processed 
    //the spawner queue
    public void Dequeue()
    {
        anim.SetTrigger("Dequeue");
        queueIndex--;
    }

    //Clear all elements in the queue UI, called in Awake method to initialise UI and by monster spawner when "Clear" message is processed
    public void ClearQueue()
    {
        foreach(RawImage element in queueElements)
        {
            element.texture = null;
        }
        queueIndex = 0;
    }

    //Called by animation event when it needs to move texture images around during dequeue
    public void DequeueTextureSwitch()
    {
        for (int i = 0; i < queueIndex; i++)
        {
            queueElements[i].texture = queueElements[i + 1].texture;
            queueElements[i + 1].texture = null;
        }
    }

    //Called by animation event when it needs to move texture images around during dequeue
    public void EnqueueTextureSwitch()
    {
        queueElements[queueIndex].texture = nextTexture;
    }
}
