using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ClaudeBridge
{
    const string CommandFile = "Temp/claude-command.txt";
    const string ResultFile = "Temp/claude-result.txt";
    const string LogFile = "Temp/claude-log.txt";

    static double nextCheck;
    static StringBuilder log;

    static ClaudeBridge()
    {
        EditorApplication.update += Tick;
        Application.logMessageReceivedThreaded -= Persist;
        Application.logMessageReceivedThreaded += Persist;
        File.WriteAllText(ResultFile, $"ready {DateTime.Now:HH:mm:ss}\n");
    }

    static void Tick()
    {
        if (EditorApplication.timeSinceStartup < nextCheck) return;
        nextCheck = EditorApplication.timeSinceStartup + 0.5;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        if (!File.Exists(CommandFile)) return;

        string[] lines = File.ReadAllLines(CommandFile);
        File.Delete(CommandFile);

        log = new StringBuilder();
        Application.logMessageReceived += Capture;
        try
        {
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;
                log.AppendLine("> " + line);
                Run(line);
            }
        }
        catch (Exception e)
        {
            log.AppendLine("EXCEPTION " + e);
        }
        finally
        {
            Application.logMessageReceived -= Capture;
            log.AppendLine($"done {DateTime.Now:HH:mm:ss}");
            File.WriteAllText(ResultFile, log.ToString());
        }
    }

    static void Run(string line)
    {
        int space = line.IndexOf(' ');
        string cmd = space < 0 ? line : line.Substring(0, space);
        string arg = space < 0 ? "" : line.Substring(space + 1).Trim();

        switch (cmd)
        {
            case "refresh":
                AssetDatabase.Refresh();
                break;
            case "call":
                Call(arg);
                break;
            case "menu":
                log.AppendLine(EditorApplication.ExecuteMenuItem(arg) ? "ok" : "menu not found: " + arg);
                break;
            case "open":
                if (EditorApplication.isPlaying)
                {
                    log.AppendLine("stop play mode first");
                    break;
                }
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene(arg, OpenSceneMode.Single);
                log.AppendLine("opened " + arg);
                break;
            case "save":
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
                log.AppendLine("saved");
                break;
            case "play":
                EditorApplication.isPlaying = true;
                break;
            case "stop":
                EditorApplication.isPlaying = false;
                break;
            case "scene":
                log.AppendLine("active scene: " + EditorSceneManager.GetActiveScene().path);
                break;
            default:
                log.AppendLine("unknown command: " + cmd);
                break;
        }
    }

    static void Call(string target)
    {
        int dot = target.LastIndexOf('.');
        if (dot < 0) { log.AppendLine("use: call ClassName.Method"); return; }
        string typeName = target.Substring(0, dot);
        string methodName = target.Substring(dot + 1);

        foreach (string assemblyName in new[] { "Assembly-CSharp-Editor", "Assembly-CSharp" })
        {
            Type type = Type.GetType(typeName + ", " + assemblyName);
            if (type == null) continue;
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, null, Type.EmptyTypes, null);
            if (method == null) { log.AppendLine("method not found: " + target); return; }
            method.Invoke(null, null);
            log.AppendLine("ok");
            return;
        }
        log.AppendLine("type not found: " + typeName);
    }

    static void Persist(string message, string stackTrace, LogType type)
    {
        try
        {
            string extra = type == LogType.Exception || type == LogType.Error ? Environment.NewLine + stackTrace : "";
            File.AppendAllText(LogFile, $"{DateTime.Now:HH:mm:ss} [{type}] {message}{extra}{Environment.NewLine}");
        }
        catch (IOException)
        {
        }
    }

    static void Capture(string message, string stackTrace, LogType type)
    {
        if (log == null) return;
        log.AppendLine($"[{type}] {message}");
    }
}
