using System;
using System.IO;
using System.Text;
using HyrphusQ.Events;
using UnityEngine;

namespace LatteGames.Log
{
    public static class LGFileLogHandler
    {
        private const int k_OneKb = 1024;
        private const int k_BufferSize = k_OneKb * 16; // 16KB char buffer to prevent performance hiccups when Flush data from char buffer to file stream
        private static LogConfig m_LogConfig;
        private static StreamWriter m_StreamWriter = null;
        private static StringBuilder m_StringBuilder = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnUnityInitialized()
        {
            if (GameDataSO.Instance.isSaveLogFile)
                Initialize(GameDataSO.Instance.logConfig);
        }

        private static void Initialize(LogConfig logConfig)
        {
            var directoryPath = $"{Application.persistentDataPath}/Log";
            var fileName = $"{Application.productName}_{Application.version}_{SystemInfo.deviceName}_{SystemInfo.deviceModel}_{DateTime.Now.ToString("ddMMHHmmsstt")}.log";
            var filePath = $"{directoryPath}/{fileName}";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, k_BufferSize);
            m_StreamWriter = new StreamWriter(fileStream)
            {
                AutoFlush = false
            };
            m_StringBuilder = new StringBuilder(k_OneKb);
            m_LogConfig = logConfig;

            // Sub events to write log to file stream & save it
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
            GameEventHandler.AddActionEvent(ApplicationLifecycleEventCode.OnApplicationPause, OnApplicationPause);
            GameEventHandler.AddActionEvent(ApplicationLifecycleEventCode.OnApplicationQuit, OnApplicationQuit);

            // Log device info
            LGDebug.Log(SystemInfo.deviceName, "Device Name");
            LGDebug.Log(SystemInfo.deviceModel, "Device Model");
        }

        private static void OnLogMessageReceived(string logMessage, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(logMessage))
                return;
            m_StreamWriter.WriteLine(FormatLogMessage(logMessage, stackTrace, type));
        }

        private static bool IsAbleToSaveStackTrace(string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(stackTrace) || stackTrace.Length <= 1)
                return false;
            var saveStackTraceLogType = m_LogConfig.saveStackTraceLogType;
            switch (type)
            {
                case LogType.Error:
                    return (saveStackTraceLogType & StackTraceLogType.Error) == StackTraceLogType.Error;
                case LogType.Assert:
                    return (saveStackTraceLogType & StackTraceLogType.Assert) == StackTraceLogType.Assert;
                case LogType.Warning:
                    return (saveStackTraceLogType & StackTraceLogType.Warning) == StackTraceLogType.Warning;
                case LogType.Log:
                    return (saveStackTraceLogType & StackTraceLogType.Log) == StackTraceLogType.Log;
                case LogType.Exception:
                    return (saveStackTraceLogType & StackTraceLogType.Exception) == StackTraceLogType.Exception;
                default:
                    return false;
            }
        }

        private static string FormatLogMessage(string logMessage, string stackTrace, LogType type)
        {
            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat("{0} - {1}: {2}", DateTime.Now, type, logMessage);
            if (IsAbleToSaveStackTrace(stackTrace, type))
                m_StringBuilder.AppendFormat("\nStackTrace:\n{0}", stackTrace.Substring(0, stackTrace.Length - 1));
            return m_StringBuilder.ToString();
        }

        private static void OnApplicationQuit()
        {
            if (m_StreamWriter == null)
                return;
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            try
            {
                m_StreamWriter.Flush();
                m_StreamWriter.Close();
                m_StreamWriter.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void OnApplicationPause()
        {
            if (m_StreamWriter == null)
                return;
            try
            {
                m_StreamWriter.Flush();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}