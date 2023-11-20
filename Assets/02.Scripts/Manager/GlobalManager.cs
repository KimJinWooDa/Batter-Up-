using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    // Singleton variable
    public static GlobalManagers Instance { get; private set; } = null;

    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }


    private void Awake()
    {
        // Singleton pattern, Why use awake method? Cuz this is the boot strap for all managers
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy the duplicate
            Destroy(gameObject);
        }
    }
}