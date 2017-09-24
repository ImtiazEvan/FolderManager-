using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;

public class FolderManagerController : MonoBehaviour 
{


     public static void UnWrapSubFolders(string directoryPath) 
     {

            float progress = 0.0f;
            EditorUtility.DisplayProgressBar("Unwraping Folders", "Working ...", progress);

            string[] allFilesPath = getAllFilesAtPath(directoryPath);

            if (allFilesPath.Length > 0) 
            {
                for (int i = 0; i < allFilesPath.Length; i++)
                {
                    string p = allFilesPath[i];

                    if(!p.Contains("meta") && AssetDatabase.ValidateMoveAsset(p, directoryPath + "/" + Path.GetFileName(p)).Length == 0) 
                    {
                        string res = AssetDatabase.MoveAsset(p, directoryPath + "/" + Path.GetFileName(p));
                        Debug.Log(res);
                    }                                       

                    progress = (float)i /(float)allFilesPath.Length * 0.8f;                    

                    EditorUtility.DisplayProgressBar("Unwraping Folders", "Working " + (int)Mathf.Ceil(progress * 100.0f) + "%", progress);                    

                }

            }



            string[] folderPaths = Directory.GetDirectories(directoryPath);

            for (int i = 0; i < folderPaths.Length; i++) 
            {
                if (folderPaths[i].Contains("Duplicates"))
                    continue;

                if (!directoryIsEmpty(folderPaths[i])) 
                {


                    if (!Directory.Exists(Path.Combine(directoryPath, "Duplicates"))) {
                        AssetDatabase.CreateFolder(directoryPath, "Duplicates");
                    }

                    string[] allfilesInFolder = Directory.GetFiles(folderPaths[i]);


                    if (!Directory.Exists(Path.Combine(directoryPath, "Duplicates/" + Path.GetFileName(folderPaths[i])))) {
                        AssetDatabase.CreateFolder(Path.Combine(directoryPath, "Duplicates"), Path.GetFileName(folderPaths[i]));
                    }


                    foreach (string subfolderFilePath in allfilesInFolder) 
                    {
                        if (!subfolderFilePath.Contains("meta")) 
                        {
                            string res = AssetDatabase.MoveAsset(subfolderFilePath, Path.Combine(directoryPath, "Duplicates/" + Path.GetFileName(folderPaths[i])) + "/" + Path.GetFileName(subfolderFilePath));
                            Debug.Log(res);
                        }                                                                        
                    }

                }

                AssetDatabase.MoveAssetToTrash(folderPaths[i]);

                progress = 0.8f + (float)i / folderPaths.Length * 0.2f;

                EditorUtility.DisplayProgressBar("Deleting Folders", "Working " + (int)Mathf.Ceil(progress * 100.0f) + "%", progress);
            }

            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();        
    }



    private static string[] getAllFilesAtPath(string path) 
    {

        Queue<string> directories = new Queue<string>();
        List<string> finalPaths = new List<string>();

        directories.Enqueue(path);


        while (directories.Count > 0) 
        {
            string p = directories.Dequeue();

            foreach (string subDirectory in Directory.GetDirectories(p)) 
            {
                directories.Enqueue(subDirectory);
            }

            if (p != path) 
            {
                finalPaths.AddRange(Directory.GetFiles(p));
            }
            
        }        

        return finalPaths.ToArray();

    }

    private static bool directoryIsEmpty(string path) 
    {
        Queue<string> t = new Queue<string>();

        t.Enqueue(path);


        while(t.Count > 0) 
        {
            string p = t.Dequeue();

            string[] paths = Directory.GetFiles(p);

            foreach (string filePath in paths) 
           {
                if (!filePath.Contains("meta")) 
                {
                    return false;
                }

            }

            foreach (string subDirectory in Directory.GetDirectories(p)) 
            {
                t.Enqueue(subDirectory);            
            }

        }
        

        return true;
    }



    private static void printArray(string[] res) 
    {
        foreach (string p in res) 
        {
            Debug.Log(p);
        }
    }
}
