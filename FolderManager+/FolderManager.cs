using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class FolderManager : EditorWindow 
{

    private string directoryPath;
    private Object activeObj;

    private int numberOfDirectories;
    private int numberOfFiles;

    struct fileStructure
    {
        public fileStructure(int x, int y) 
        {
            this.NumberOfFiles = x;
            this.NumberOfDirectory = y;

        }

        public int NumberOfFiles;
        public int NumberOfDirectory;
    }


    [MenuItem("Window/Folder Manager")]
    public static void showWindow() 
    {
        EditorWindow.GetWindow(typeof(FolderManager));
    }

    void Awake() 
    {
        numberOfDirectories = 0;
        numberOfFiles = 0;
        directoryPath = "";


        var objects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        if(objects.Length > 0) 
        {

            activeObj = objects[0];


            string currentdirectoryPath = AssetDatabase.GetAssetPath(activeObj.GetInstanceID());


            if (Directory.Exists(currentdirectoryPath)) {
                directoryPath = currentdirectoryPath;
            }
            else {
                directoryPath = Path.GetDirectoryName(currentdirectoryPath);
            }

            fileStructure fs = getFilesAndDirectiories(directoryPath);

            numberOfDirectories = fs.NumberOfDirectory;
            numberOfFiles = fs.NumberOfFiles;

        }
        
    }


    void OnGUI()
    {
        /*Object obj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)[0];
        string currentdirectoryPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());*/                

        EditorGUI.TextField(new Rect(5,25,position.width-10,16), "Current Directory", directoryPath);
        EditorGUI.HelpBox(new Rect(5, 45, position.width - 10, 16), "has " + numberOfDirectories + " directories & " + numberOfFiles + " files",MessageType.None);

        if(GUI.Button(new Rect(5,65,position.width/2-10,20),"UnWrap Files")) 
        {
            unWrapFolders();
        }

        if (GUI.Button(new Rect(position.width / 2 + 5, 65, position.width / 2 - 10, 20), "Init Directories")) 
        {
            Debug.Log("hello");
        }


    }

    void Update() 
    {

        var objects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        if(objects.Length > 0) 
        {

            Object selectedObj = objects[0];

            if (activeObj != selectedObj) {
                activeObj = selectedObj;

                string currentdirectoryPath = AssetDatabase.GetAssetPath(activeObj.GetInstanceID());

                if (Directory.Exists(currentdirectoryPath)) {
                    directoryPath = currentdirectoryPath;
                }
                else {
                    directoryPath = Path.GetDirectoryName(currentdirectoryPath);
                }


                fileStructure fs = getFilesAndDirectiories(directoryPath);

                numberOfDirectories = fs.NumberOfDirectory;
                numberOfFiles = fs.NumberOfFiles;

            }

            Repaint();
        }       
        

    }



    private static fileStructure getFilesAndDirectiories(string path)
    {
        int files = 0;
        int directories = 0;


        Queue<string> t = new Queue<string>();
        t.Enqueue(path);


        while(t.Count > 0) 
        {
            string p = t.Dequeue();

            foreach (string fileName in Directory.GetFiles(p)) 
            {
                if (!fileName.Contains("meta")) 
                {
                    files++;
                }
            }


            foreach(string subDirectory in Directory.GetDirectories(p)) 
            {

                directories++;
                t.Enqueue(subDirectory);
            }
        }

        return new fileStructure(files,directories);
    }


    private void unWrapFolders() 
    {
        FolderManagerController.UnWrapSubFolders(directoryPath);
    }

}
