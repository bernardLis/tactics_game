using System;
using System.IO;
using UnityEngine;


//https://github.com/UnityTechnologies/UniteNow20-Persistent-Data/blob/main/FileManager.cs
//C:\Users\blis\AppData\LocalLow\DefaultCompany\Tactics Bu
namespace Lis.Core.Utilities
{
    public static class FileManager
    {
        public static bool CreateFile(string fileName)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                File.WriteAllText(fullPath, "");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }

        public static bool FileExists(string fileName)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);
            return File.Exists(fullPath);
        }

        public static bool WriteToFile(string fileName, string fileContents)
        {
            if (fileName == null)
                return false;

            if (!FileExists(fileName))
                CreateFile(fileName);

            var fullPath = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                File.WriteAllText(fullPath, fileContents);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out string result)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                result = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read from {fullPath} with exception {e}");
                result = "";
                return false;
            }
        }

        public static bool DeleteFile(string fileName)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                File.Delete(fullPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete {fullPath} with exception {e}");
                return false;
            }
        }
    }
}
