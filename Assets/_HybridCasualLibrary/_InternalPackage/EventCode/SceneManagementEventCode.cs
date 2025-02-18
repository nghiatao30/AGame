using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum SceneManagementEventCode
{
    /// <summary>
    /// This event is raised before starting the process of loading a scene.
    /// <para> <typeparamref name="string"/>: destinationSceneName </para>
    /// <para> <typeparamref name="string"/>: originSceneName </para>
    /// <para> <typeparamref name="LoadSceneRequest"/>: request </para>
    /// </summary>
    OnStartLoadScene,
    /// <summary>
    /// This event is raised after starting the process of loading a scene.
    /// <para> <typeparamref name="string"/>: destinationSceneName </para>
    /// <para> <typeparamref name="string"/>: originSceneName </para>
    /// <para> <typeparamref name="LoadSceneRequest"/>: request </para>
    /// <para> <typeparamref name="LoadSceneAsyncTask"/>: loadSceneAsyncTask (it will be null if we call LoadScene instead of LoadSceneAsync) </para>
    /// </summary>
    OnLoadSceneStarted,
    /// <summary>
    /// This event is raised after a scene has been successfully loaded.
    /// <para> <typeparamref name="string"/>: destinationSceneName </para>
    /// <para> <typeparamref name="string"/>: originSceneName </para>
    /// <para> <typeparamref name="LoadSceneRequest"/>: request </para>
    /// </summary>
    OnLoadSceneCompleted,
    /// <summary>
    /// This event is raised before starting the process of unloading a scene.
    /// <para> <typeparamref name="string"/>: sceneName </para>
    /// <para> <typeparamref name="LoadSceneRequest"/>: request </para>
    /// </summary>
    OnStartUnloadScene,
    /// <summary>
    /// This event is raised after starting the process of unloading a scene.
    /// <para> <typeparamref name="string"/>: sceneName </para>
    /// <para> <typeparamref name="LoadSceneRequest"/>: request </para>
    /// <para> <typeparamref name="UnloadSceneAsyncTask"/>: unloadSceneAsyncTask </para>
    /// </summary>
    OnUnloadSceneStarted,
    /// <summary>
    /// This event is raised after a scene has been successfully unloaded.
    /// <para> <typeparamref name="string"/>: sceneName </para>
    /// <para> <typeparamref name="LoadSceneRequest"/>: request </para>
    /// </summary>
    OnUnloadSceneCompleted
}