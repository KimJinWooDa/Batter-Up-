using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    private List<AudioSourceWrapper> audioSources;
    private int currentIndex = 0;

    public AudioSourcePool(int poolSize, GameObject audioSourcePrefab)
    {
        audioSources = new List<AudioSourceWrapper>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            var audioSourceGameObject = Instantiate(audioSourcePrefab);
            var wrapper = audioSourceGameObject.GetComponent<AudioSourceWrapper>();
            audioSources.Add(wrapper);
        }
    }

    public AudioSourceWrapper GetAvailableSource()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            int index = (currentIndex + i) % audioSources.Count;
            if (!audioSources[index].IsPlaying)
            {
                currentIndex = (index + 1) % audioSources.Count;
                return audioSources[index];
            }
        }
        return null;
    }

}