using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Singleton<T> : MonoBehaviour where T : Component
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {                
                instance = FindObjectOfType<T>(); // Search for existing instance
            }            
            if (!instance) // Create new instance if one doesn't already exist
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }
}