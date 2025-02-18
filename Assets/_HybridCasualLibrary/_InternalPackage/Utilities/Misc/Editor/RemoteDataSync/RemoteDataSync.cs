using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class RemoteDataSync
{
    public const string Title = "Sync Remote Data";
    public const string SuccessMessage = "Sync Succeeded Bruh :))";
    public const string FailMessage = "Sync Failed Bruh :'(";
    public const string OkMessage = "OK";
    public const string CancelMessage = "Cancel";

    private static UnityWebRequest SyncInternal(string remoteUrl, string localFilePath, Action<bool> callback = null)
    {
        var webRequest = UnityWebRequest.Get(remoteUrl);
        var asyncOperation = webRequest.SendWebRequest();
        asyncOperation.completed += OnCompleted;

        void OnCompleted(AsyncOperation asyncOperation)
        {
            bool isSucceeded = false;
            try
            {
                isSucceeded = string.IsNullOrEmpty(webRequest.error);
                if (isSucceeded)
                {
                    switch (webRequest.result)
                    {
                        case UnityWebRequest.Result.Success:
                            var absoluteFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), localFilePath);
                            var absoluteDirectoryPath = Path.GetDirectoryName(absoluteFilePath);
                            if (!Directory.Exists(absoluteDirectoryPath))
                                Directory.CreateDirectory(absoluteDirectoryPath);
                            if (!File.Exists(absoluteFilePath))
                            {
                                var fileStream = File.Create(absoluteFilePath);
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                            File.WriteAllText(absoluteFilePath, webRequest.downloadHandler.text);

                            AssetDatabase.Refresh();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"Failed: {webRequest.error} - Maybe there is a problem with your internet-connection Bruh");
                }
            }
            catch (Exception e)
            {
                isSucceeded = false;
                Debug.LogException(e);
            }
            // Notify event
            callback?.Invoke(isSucceeded);
        }

        return webRequest;
    }

    public static UnityWebRequest Sync(string remoteUrl, string localFilePath, bool showDialog = true, Action<bool> callback = null)
    {
        UnityWebRequest webRequest = null;
        webRequest = SyncInternal(remoteUrl, localFilePath, OnCompleted);

        void OnCompleted(bool isSucceeded)
        {
            // Notify event
            callback?.Invoke(isSucceeded);

            // Show diaglog
            if (showDialog)
            {
                if (isSucceeded)
                {
                    EditorUtility.DisplayDialog(Title, SuccessMessage, OkMessage);
                }
                else
                {
                    EditorUtility.DisplayDialog(Title, FailMessage, OkMessage);
                }
            }

            webRequest.Dispose();
        }

        return webRequest;
    }

    public static void Sync(string[] remoteUrls, string[] localFilePaths, bool showDialog = true, Action<bool> callback = null)
    {
        if (remoteUrls == null || localFilePaths == null || remoteUrls.Length != localFilePaths.Length)
        {
            Debug.LogError("Bruh ???");
            return;
        }
        var isFailed = false;
        var succeedSyncDataCount = 0;
        var requestQueue = new Queue<UnityWebRequest>();
        for (int i = 0; i < remoteUrls.Length; i++)
        {
            requestQueue.Enqueue(SyncInternal(remoteUrls[i], localFilePaths[i], OnCompleted));
        }

        float CalcProgress()
        {
            float progress = 0f;
            foreach (var request in requestQueue)
            {
                progress += request.downloadProgress;
            }
            return progress / requestQueue.Count;
        }
        void OnCompleted(bool isSucceeded)
        {
            if (isFailed)
                return;
            isFailed = !isSucceeded;
            if (isSucceeded)
            {
                var progress = CalcProgress();
                EditorUtility.DisplayProgressBar("Sync Data In-Progress", $"{progress * 100}%", progress);

                if (++succeedSyncDataCount == remoteUrls.Length)
                {
                    foreach (var request in requestQueue)
                    {
                        request.Abort();
                        request.Dispose();
                    }
                    EditorUtility.ClearProgressBar();

                    // Notify event
                    callback?.Invoke(true);

                    // Show diaglog
                    if (showDialog)
                    {
                        EditorUtility.DisplayDialog(Title, SuccessMessage, OkMessage);
                    }
                }
            }
            else
            {
                foreach (var request in requestQueue)
                {
                    request.Abort();
                    request.Dispose();
                }
                EditorUtility.ClearProgressBar();

                // Notify event
                callback?.Invoke(false);

                // Show diaglog
                if (showDialog)
                {
                    EditorUtility.DisplayDialog(Title, FailMessage, OkMessage);
                }
            }
        }
    }
}