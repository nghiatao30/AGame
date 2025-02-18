using UnityEngine;
using HyrphusQ.Events;

public class ApplicationLifecycleEventNotifier : MonoBehaviour
{
    private void OnApplicationFocus(bool focus)
    {
        GameEventHandler.Invoke(ApplicationLifecycleEventCode.OnApplicationFocus, focus);
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        GameEventHandler.Invoke(ApplicationLifecycleEventCode.OnApplicationPause, pauseStatus);
    }
    private void OnApplicationQuit()
    {
        GameEventHandler.Invoke(ApplicationLifecycleEventCode.OnApplicationQuit);
    }
}