using UnityEngine;
using UnityEngine.UI;
using MyExceptions;
using System.Collections.Generic;

//Class that implements the IQueueUI interface, siomple implementation that hides and changes the textures of raw images to show a list of messages in the queue from right 
//to left, alternative ti the animated queue UI
public class StaticQueueUI : MonoBehaviour, IQueueUI
{
    //Game objects reperesenting each of the elements in the queue UI
    [SerializeField]
    List<GameObject> queueElements = new List<GameObject>();

    //Raw images displaying the queue element
    [SerializeField]
    List<RawImage> queueImages = new List<RawImage>();

    //Working index in the queue, equal to the number of active elements in the queue
    public int queueIndex = 0, maxQueueSize = 0;

    //Public readonly property for maxQueueSize so that the QueuedMonsterSpawner can check how many elements the UI can show
    public int MaxQueueSize { get { return maxQueueSize; } }

    private void Awake()
    {
        //Initialise UI elements
        ClearQueue();

        //Set max queue size to whichever is lower: the number of available queue element game objects or the number of available queue raw images
        maxQueueSize = queueElements.Count > queueImages.Count ? queueImages.Count : queueElements.Count;
    }

    //Add new image to the end of the queue UI, only called by the monster spawner when a new message is added to the spawner queue
    public void Enqueue(Texture newElement)
    {
        //Check if the maximum queue size has been reached, if it hasn't add new element to the end and increment queue index, else throw error
        if (queueIndex + 1 < maxQueueSize)
        {
            queueImages[queueIndex].texture = newElement;
            queueElements[queueIndex].SetActive(true);
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
        //Check if the maximum queue size has been reached, if it hasn't add new element to the front and increment queue index, else throw error
        if (queueIndex + 1 < maxQueueSize)
        {
            queueElements[queueIndex].SetActive(true);
            for(int i = queueIndex; i > 0; i--)
            {
                queueImages[i].texture = queueImages[i - 1].texture;
            }
            queueImages[queueIndex].texture = newElement;
            queueIndex++;
        }
        else
        {
            throw new QueueSizeException();
        }
    }

    //If queue isn't empty shift all message images down one queue element and disable the the last activated element, else throw an error
    public void Dequeue()
    {
        if (queueIndex > 0)
        {
            queueIndex--;
            for (int i = 0; i < queueIndex; i++)
            {
                queueImages[i].texture = queueImages[i + 1].texture;
            }
            queueElements[queueIndex].SetActive(false);
            queueImages[queueIndex].texture = null;
        }
        else
        {
            throw new QueueSizeException("No elements to dequeue");
        }
    }

    //Clear all elements in the queue UI, called in Awake method to initialise UI and by monster spawner when "Clear" message is processed
    public void ClearQueue()
    {
        //Set all queue image textures to null and de-activate all queue element game objects to initialse the queue UI
        foreach (RawImage image in queueImages)
        {
            image.texture = null;
        }

        foreach (GameObject element in queueElements)
        {
            element.SetActive(false);
        }

        queueIndex = 0;
    }

}
