using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using System;

[CreateAssetMenu(fileName = "Sound_Library_SO", menuName = "LatteGames/Sound/Sound Library SO", order = 0)]
public class SoundLibrarySO : SerializedScriptableObject
{
    const string extension = ".cs";
    [FolderPath]
    public string m_EnumScriptsPath = "/Assets";

    [DictionaryDrawerSettings(KeyLabel = "Enum string", ValueLabel = "Audio clip")]
    public Dictionary<string, AudioClip> Library;

#if UNITY_EDITOR
    [SerializeField]
    string m_SheetID;
    public string SheetID => m_SheetID;
    [SerializeField]
    string m_SheetName;
    public string SheetName => m_SheetName;

    public void AddToDictionary(string key)
    {
        if (Library == null)
            Library = new Dictionary<string, AudioClip>();

        string[] recordNameParsed = key.Split(".");

        string enumKey = $"{recordNameParsed[0]}.{recordNameParsed[1]}";

        if (recordNameParsed.Length >= 3)
            enumKey = enumKey + recordNameParsed[2]; //reformat to fit enum
        var guid = AssetDatabase.FindAssets($"t:AudioClip {key}");
        AudioClip audioClip = null;
        foreach (var i in guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(i);
            audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            Debug.Log(path);
        }
        Library.Set(enumKey, audioClip);
    }

    /// <summary>
    /// Save the dictionary keys into an enum for simplified uses.
    /// </summary>
    [Button("Save list to enum")]
    private void SaveToEnumButton()
    {
        foreach (var i in Library)
        {
            if (i.Key.ToString().Length == 0 || i.Key.ToString().Split(".").Length <= 1)
            {
                Debug.Log("Invalid enum name. Input enum must following this format EnumName.EnumElementName, e.g: SFX.ButtonClick");
                return;
            }

            string enumFileName = i.Key.ToString().Split(".")[0];
            string enumName = i.Key.ToString().Split(".")[1];
            WriteToEnum(m_EnumScriptsPath, enumFileName, enumName);
        }
    }

    public void RemoveEnum<T>(string filePath, string fileName, T data)
    {
        if (File.Exists(filePath + "/" + fileName + extension))
        {
            string fileContent = "";
            if (fileName.Length == 0 || data.ToString().Length == 0)
            {
                return;
            }
            try
            {
                using (StreamReader sr = new StreamReader(filePath + "/" + fileName + extension))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Contains(data.ToString()) && line.Length > 0)
                            fileContent += line + "\n";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            StringReader strReader = new StringReader(fileContent);
            string fileContentLine = "";
            using (StreamWriter file = File.CreateText(filePath + "/" + fileName + extension))
            {
                while ((fileContentLine = strReader.ReadLine()) != null)
                {
                    file.WriteLine(fileContentLine);
                }
            }
            AssetDatabase.Refresh();
        }
    }

    public void WriteToEnum<T>(string filePath, string fileName, T data)
    {
        if (File.Exists(filePath + "/" + fileName + extension))
        {
            string fileContent = "";
            if (data.ToString().Length == 0)
            {
                Debug.LogError("Input enum must not be emtpy");
                return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(filePath + "/" + fileName + extension))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var splittedEnum = line.Split("=");
                        if (splittedEnum[0].Replace(" ", string.Empty).Trim().Equals(data.ToString().Trim()))
                        {
                            Debug.LogWarning($"{data.ToString().Trim()} skipped due exist");
                            return; //skip due exist;
                        }
                        if (!line.Equals("}") && line.Length > 0)
                            fileContent += line + "\n";
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            var newEnumLine = "";
            string lineRep = data.ToString().Replace(" ", string.Empty);

            StringReader strReader = new StringReader(fileContent);
            string fileContentLine = "";
            if (strReader.ReadLine().CaseInsensitiveContains("[soundid]")) //if file content is not empty, append new enum
            {
                int newEnumIndex = 0; //determine new enum index
                while ((fileContentLine = strReader.ReadLine()) != null)
                {
                    if (fileContentLine.Contains("public") || fileContentLine.Contains("{") || fileContentLine.Contains("}"))
                    {
                        continue;
                    }
                    else
                    {
                        newEnumIndex++;
                    }
                }
                if (!string.IsNullOrEmpty(lineRep))
                {
                    newEnumLine = string.Format("    {0} = {1},", lineRep, newEnumIndex);
                }
                fileContent += newEnumLine + "\n}";
                StringReader rewriteStrReader = new StringReader(fileContent);
                using (StreamWriter file = File.CreateText(filePath + "/" + fileName + extension))
                {
                    while ((fileContentLine = rewriteStrReader.ReadLine()) != null)
                    {
                        file.WriteLine(fileContentLine);
                    }
                    file.Close();
                }
            }
            else
            {
                Debug.Log("File content is empty, add new enum");
                //if file content is empty, add new enum
                using (StreamWriter file = new StreamWriter(filePath + "/" + fileName + extension, true))
                {
                    file.WriteLine("[SoundID]");
                    file.WriteLine("public enum " + fileName + "\n{");
                    if (!string.IsNullOrEmpty(lineRep))
                    {
                        file.WriteLine(string.Format("    {0} = {1},", lineRep, 0));
                    }

                    file.Write("}");
                    file.Close();
                }
            }

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("Create new file");
            using (StreamWriter file = new StreamWriter(filePath + "/" + fileName + extension, true))
            {
                file.WriteLine("[SoundID]");
                file.WriteLine("public enum " + fileName + "\n{");

                string lineRep = data.ToString().Replace(" ", string.Empty);
                if (!string.IsNullOrEmpty(lineRep))
                {
                    file.WriteLine(string.Format("    {0} = {1},",
                        lineRep, 0));
                }

                file.Write("}");
                file.Close();
            }

            AssetDatabase.ImportAsset(filePath + "/" + fileName + extension);
            AssetDatabase.Refresh();
        }
    }
#endif
}



