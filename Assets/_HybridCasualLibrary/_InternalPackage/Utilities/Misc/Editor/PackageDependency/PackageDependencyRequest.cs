using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public static class PackageDependencyRequest
{
    [InitializeOnLoadMethod]
    private static void CheckMissingDependencies()
    {
        EditorApplication.delayCall += DelayCall;

        void DelayCall()
        {
            if (PackageDependencySO.Instance == null)
                return;

            SearchMissingDependencies(packagesToAdd =>
            {
                if (packagesToAdd.Count > 0)
                {
                    if (EditorUtility.DisplayDialog("Missing Dependencies", "It looks like you're missing some *Package Dependencies*.\nPlease add it in order to make it work properly!!!", "Add pls!", "No???"))
                    {
                        AddDependencies(packagesToAdd);
                    }
                }
            });
            EditorApplication.delayCall -= DelayCall;
        }
    }

    [MenuItem("LatteGames/Update Dependencies")]
    private static void UpdateMissingDependencies()
    {
        SearchMissingDependencies(packagesToAdd =>
        {
            if (packagesToAdd.Count > 0)
            {
                AddDependencies(packagesToAdd);
            }
            else
            {
                EditorUtility.DisplayDialog("Catch up", "All package dependencies have already been added Bruh!!!", "OK");
            }
        });
    }

    private static void SearchMissingDependencies(Action<List<PackageDependencySO.Package>> callback)
    {
        var packageDependencySO = PackageDependencySO.Instance;
        var searchPackagesRequest = Client.List();
        EditorApplication.update += SearchProgress;

        void SearchProgress()
        {
            if (searchPackagesRequest.IsCompleted)
            {
                if (searchPackagesRequest.Status == StatusCode.Success)
                {
                    var packagesToAdd = new List<PackageDependencySO.Package>(packageDependencySO.packageDependencies);
                    foreach (var package in searchPackagesRequest.Result)
                    {
                        var existedPackage = packagesToAdd.Find(item => item.packageName == package.name);
                        if (existedPackage != null)
                        {
                            packagesToAdd.Remove(existedPackage);
                            continue;
                        }
                    }
                    callback?.Invoke(packagesToAdd);
                }
                else if (searchPackagesRequest.Status >= StatusCode.Failure)
                    Debug.LogError(searchPackagesRequest.Error.message);

                EditorApplication.update -= SearchProgress;
            }
        }
    }

    private static void AddDependencies(List<PackageDependencySO.Package> packagesToAdd)
    {
        var progress = 0f;
        var packageDependencySO = PackageDependencySO.Instance;
        var addAndRemovePackagesRequest = Client.AddAndRemove(packagesToAdd.Select(package => package.packageId).ToArray());
        EditorApplication.update += AddProgress;

        void AddProgress()
        {
            // There is no way to get progress from request just show fake progress bar to indicate it's is running.
            progress = Mathf.Clamp01(progress + 1f / (60f * 15f));
            EditorUtility.DisplayProgressBar("Add package dependencies", $"In Progress {Mathf.RoundToInt(progress * 90)}/100%", progress * 0.9f);
            if (addAndRemovePackagesRequest.IsCompleted)
            {
                if (addAndRemovePackagesRequest.Status == StatusCode.Success)
                {
                    foreach (var package in addAndRemovePackagesRequest.Result)
                    {
                        if (packageDependencySO.packageDependencies.Exists(item => item.packageId == package.packageId))
                        {
                            Debug.Log("Add package name: " + package.name);
                        }
                    }
                    EditorUtility.DisplayDialog("Succeeded", "Add Package Dependencies successfully Bruh!!!", "OK");
                }
                else if (addAndRemovePackagesRequest.Status == StatusCode.Failure)
                    Debug.LogError(addAndRemovePackagesRequest.Error.message);

                EditorUtility.ClearProgressBar();
                EditorApplication.update -= AddProgress;
            }
        }
    }
}