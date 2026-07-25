using UnityEditorInternal;
using UnityEngine;

public class PersistentRoot : MonoBehaviour
{
    private static PersistentRoot _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
