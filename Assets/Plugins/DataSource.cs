using System.IO;
using UnityEngine;

// persistent data and streaming assets data is seperated to secure live data
// for testing and during development

public enum DataSource
{
    Default, // uses editor if in editor, otherwise release
    Release,
    Editor,
}

public static class DataSourceUtils
{
    public static string GetDataDirectory(this DataSource source, string appendDirectory = null)
    {
        return GetDirectory(source, Application.persistentDataPath, appendDirectory);
    }

    public static string GetAssetsDirectory(this DataSource source, string appendDirectory = null)
    {
        return GetDirectory(source, Application.streamingAssetsPath, appendDirectory);
    }

    private static string GetDirectory(DataSource source, string root, string appendDirectory)
    {
        string dataDir = "Editor";
        if (source == DataSource.Release || source == DataSource.Default && !Application.isEditor)
        {
            dataDir = "Release";
        }
        string directory = Path.Combine(root, dataDir);
        if (!string.IsNullOrEmpty(appendDirectory))
        {
            directory = Path.Combine(directory, appendDirectory);
        }
        Directory.CreateDirectory(directory);
        return directory;
    }
}