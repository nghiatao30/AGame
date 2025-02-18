using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using CsvHelper;

[CreateAssetMenu(fileName = "AudioDatasheet", menuName = "LatteGames/Editor/Datasheet/AudioDatasheet")]
public class AudioDatasheet : Datasheet
{
    public class AudioEditorData
    {
        [field: SerializeField]
        public string sheetID { get; set; }
    }

    [SerializeField]
    protected Dictionary<SoundLibrarySO, AudioEditorData> m_AudioEditorDataDictionary;

    public override void ExportData(string directoryPath)
    {
        Debug.LogError("Not support!");
    }

    public override void ImportData()
    {
        var remoteSheetUrls = new List<string>();
        var localFilePaths = new List<string>();
        foreach (var item in m_AudioEditorDataDictionary)
        {
            remoteSheetUrls.Add(remotePath.Replace("{sheetID}", item.Key.SheetID));
            localFilePaths.Add(localPath.Replace("{name}", item.Key.SheetName));
        }
        RemoteDataSync.Sync(remoteSheetUrls.ToArray(), localFilePaths.ToArray(), false, OnSyncCompleted);

        void OnSyncCompleted(bool isSucceeded)
        {
            if (!isSucceeded)
            {
                EditorUtility.DisplayDialog(RemoteDataSync.Title, RemoteDataSync.FailMessage, RemoteDataSync.OkMessage);
                return;
            }
            foreach (var item in m_AudioEditorDataDictionary)
            {
                if (string.IsNullOrEmpty(item.Key.SheetID) || string.IsNullOrEmpty(item.Key.SheetName))
                    continue;
                var localFilePath = localPath.Replace("{name}", item.Key.SheetName);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    IgnoreBlankLines = true,
                    MissingFieldFound = null,
                };
                using (var reader = new StreamReader(localFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        string recordName = csv.GetField("NAME");
                        item.Key.AddToDictionary(recordName);
                    }
                }
            }
            EditorUtility.DisplayDialog(RemoteDataSync.Title, RemoteDataSync.SuccessMessage, RemoteDataSync.OkMessage);
        }
    }
}