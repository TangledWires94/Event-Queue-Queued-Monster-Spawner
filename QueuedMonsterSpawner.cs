using System.Collections.Generic;
using UnityEngine;

//Class that implements a queued message handler to control spawning monsters in the order that a user requests them and only once a new monster is allowed to be spawned
public class QueuedMonsterSpawner : MonoBehaviour
{
    //Dictionary of objects to spawn
    Dictionary<string, TriggerRespawn> spawnablesDictionary = new Dictionary<string, TriggerRespawn>();
    
    //spawnablesDictionary components
    [Header("Dictionaries")]
    [SerializeField]
    string[] spawnablesNames = new string[0];
    [SerializeField]
    TriggerRespawn[] spawnablesObjects = new TriggerRespawn[0];

    //Dictionary of queue UI textures, one for each message type
    Dictionary<string, Texture> messageTexturesDictionary = new Dictionary<string, Texture>();

    //messageTexturesDictionary components
    [SerializeField]
    string[] messageTypes = new string[0];
    [SerializeField]
    Texture[] messageTextures = new Texture[0];

    [Header("Message Queue")]
    [SerializeField]
    int queueSize = 0;

    //Struct that defines the type that is stored within the message queue, used a struct (value type) rather than a class (reference type) because elements in the queue will 
    //be allocated and de-allocated frequently and value types are more lightweight than reference types. The SpawnerMessage type will not be converted or have it's data 
    //changed after creation and it is a basic type designed to store data so there is no need for it to be a class.
    [SerializeField]
    public struct SpawnerMessage
    {
        /*
        Spawner message is made up of two components;
            - Enum that defines the message type, used by the message handler to select which handling code to run
            - Data variable of base object class so that any data type that inherits from object (which is almost all of them) can be used, message handler knows what type 
              the data will be so errors will only occur if messages are enqueued incorrectly
        */

        public enum Message { nullMessage, spawnObject, clearQueue };
        public Message message;
        public object data;
        public SpawnerMessage(Message message, object data = null)
        {
            this.message = message;
            this.data = data;
        }

        //Created override of ToString() to help with debugging
        public override string ToString()
        {
            return message.ToString() + " : " + data.ToString();
        }
    }

    //The message queue itself
    List<SpawnerMessage> messageQueue = new List<SpawnerMessage>();

    //Can't serialize interfaces to allow assigning them via drag and drop in the inspector, instead going to have serialized field for the GameObject holding the interface 
    //and find interface reference through that
    [SerializeField]
    GameObject queueUIObject = null;
    IQueueUI queueUI = null;

    [Header("Spawner Parameters")]
    //Point at which any monsters/objects spawned by this spawner will be instantiated at
    [SerializeField]
    Vector3 spawnPosition = Vector3.zero;

    [SerializeField]
    bool readyToSpawn = true;

    private void Awake()
    {
        //Cannot build dictionary in editor so build dictionaries from elements in each component arrays
        for(int i = 0; i < spawnablesNames.Length; i++)
        {
            spawnablesDictionary.Add(spawnablesNames[i], spawnablesObjects[i]);
        }

        for (int i = 0; i < messageTypes.Length; i++)
        {
            messageTexturesDictionary.Add(messageTypes[i], messageTextures[i]);
        }

        //Find reference to the component that implements the queue UI interface on the queue UI game object
        queueUI = queueUIObject.GetComponent<IQueueUI>();
        try
        {
            //Check if the number of elements in the message queue is less than the available elements in the queue UI, if it isn't set the max message queue size
            //to be the same as the max UI queue size
            if (queueSize > queueUI.MaxQueueSize)
            {
                queueSize = queueUI.MaxQueueSize;
            }
        }
        catch (System.NullReferenceException)
        {
            //If there is no queue UI component present, write a warning to the console and create a new NullQueueUI object to prevent errors from ocurring when the code tries 
            //to access the queueUI
            Debug.Log("No component that implements IQueueUI attached to the game object");
            queueUI = new NullQueueUI();
            queueSize = queueUI.MaxQueueSize;
        }
    }

    private void Update()
    {
        //Debug code, prints the current contents of the queue to the console
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PrintQueue();
        }

        //If there is an element currently in the queue, dequeue it and run the appropriate message handling code
        if(messageQueue.Count > 0)
        {
            //Waiut while the next message is a spawn message and the spawner is not ready to spawn another object
            if(messageQueue[0].message == SpawnerMessage.Message.spawnObject && !readyToSpawn)
            {
                return;
            }

            //Dequeue the next message from the queue and run the appropriate message handling code based on the value of the message enum, because the messages are structs 
            //the messages must have data to be processed
            SpawnerMessage nextMessage = Dequeue();
            switch (nextMessage.message)
            {
                //Null message, do nothing
                case SpawnerMessage.Message.nullMessage:
                    break;

                //Spawn the requested object by gettign its name and looking up the corresponding gameobject from the dictionary and updating the queue UI
                case SpawnerMessage.Message.spawnObject:
                    string name = (string)nextMessage.data;
                    TriggerRespawn objectToSpawn = spawnablesDictionary[name];
                    if (objectToSpawn != null)
                    {
                        SpawnObject(spawnPosition, objectToSpawn);
                    }
                    queueUI.Dequeue();
                    break;

                //Clear the queue of any messages
                case SpawnerMessage.Message.clearQueue:
                    messageQueue = new List<SpawnerMessage>();
                    queueUI.ClearQueue();
                    break;
            }
        }
    }

    //Spawn the requested object in the scene and subscribe to its respawn trigger event so that the next monster can be spawned at the correct time
    void SpawnObject(Vector3 spawnPosition, TriggerRespawn objectToSpawn)
    {
        TriggerRespawn newObject = Instantiate(objectToSpawn, spawnPosition, objectToSpawn.transform.rotation);
        newObject.triggerRespawn += EnableRespawn;
        readyToSpawn = false;
    }

    void EnableRespawn()
    {
        readyToSpawn = true;
    }

    //Add new message to the end of the queue and update the queue UI to show the new message that has been added
    public void Enqueue(SpawnerMessage spawnerMessage)
    {
        if(messageQueue.Count < queueSize)
        {
            messageQueue.Add(spawnerMessage);
            queueUI.Enqueue(messageTexturesDictionary[spawnerMessage.data.ToString()]);
        }
    }

    //Same as enqueue but new message is added to the front so that it is processed next
    public void PriorityEnqueue(SpawnerMessage spawnerMessage)
    {
        if (messageQueue.Count < queueSize)
        {
            messageQueue.Insert(0, spawnerMessage);
            queueUI.PriorityEnqueue(messageTexturesDictionary[spawnerMessage.data.ToString()]);
        }
    }

    //Remove and return the next message from the start of the list, move all other elements down to fill in the gap
    SpawnerMessage Dequeue()
    {
        if(messageQueue.Count <= 0)
        {
            //If the queue is empty and this been called return a null message so that nothing happens in the scene but the code can continue
            return new SpawnerMessage(SpawnerMessage.Message.nullMessage);
        }
        else
        {
            SpawnerMessage nextMessage = messageQueue[0];
            messageQueue.RemoveAt(0);
            return nextMessage;
        }
    }

    //Used during debugging, prints a string representation of the messages in the queue to the console in order
    void PrintQueue()
    {
        for(int i = 0; i < messageQueue.Count; i++)
        {
            Debug.Log(messageQueue[i].ToString());
        }
    }
}
