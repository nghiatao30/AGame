using System.Collections;
using UnityEngine;

namespace LatteGames
{
public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner CreateCoroutineRunner(
        bool dontDestroyOnLoad
    ){
        var gameObjectHolder = new GameObject("Coroutine-Runner");
        if(dontDestroyOnLoad)
            DontDestroyOnLoad(gameObjectHolder);
        return gameObjectHolder.AddComponent<CoroutineRunner>();
    }

    public enum InteruptBehaviour
    {
        Replace,
        Ignore
    }

    private Coroutine currentCoroutine = null;
    private bool isRunning;
    public bool IsRunning => isRunning;

    public void StartManagedCoroutine(IEnumerator iEnumerator, InteruptBehaviour interuptBehaviour)
    {
        if(currentCoroutine != null && isRunning)
        {
            switch (interuptBehaviour)
            {
                case InteruptBehaviour.Replace:
                    StopManagedCoroutine();
                    break;
                case InteruptBehaviour.Ignore:
                    return;
            }
        }
        currentCoroutine = StartCoroutine(ManagedCoroutineWrapper(iEnumerator));
    }

    private IEnumerator ManagedCoroutineWrapper(IEnumerator iEnumerator)
    {
        isRunning = true;
        yield return iEnumerator;
        isRunning = false;
    }

    public void StopManagedCoroutine()
    {
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        isRunning = false;
    }
}
}