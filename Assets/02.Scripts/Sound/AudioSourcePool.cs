using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviourSingleton<AudioSourcePool>
{
    [SerializeField] private Queue<GameObject> availableObjects = new Queue<GameObject>();
    [SerializeField] private List<GameObject> debugAvailableObjectsList;
    private new void Awake()
    {
        Create(10);
        UpdateDebugList();
    }
    
    private void UpdateDebugList()
    {
        debugAvailableObjectsList = new List<GameObject>(availableObjects);
    }
    
    private void Create(int initialBufferSize)
    {
        for (int i = 0; i < initialBufferSize; i++)
        {
            GameObject obj = new GameObject("PooledAudioSource");
            obj.SetActive(false); 
            availableObjects.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (availableObjects.Count == 0)
        {
            Debug.LogWarning("Pool ran out! Increasing size.");
            GameObject obj = new GameObject("PooledAudioSource");
            obj.AddComponent<AudioSource>();
            return obj;
        }

        GameObject pooledObject = availableObjects.Dequeue();
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        availableObjects.Enqueue(obj);
        UpdateDebugList();
    }
}