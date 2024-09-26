using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
//using ClickToFormat;

public class ProjectNotesEditor : EditorWindow
{

    [SerializeField]
    private GUISkin skinDark;
    private GUISkin skinLight;
    private GUISkin skin;
    private Vector2 scrollPos;
    private Vector2 scrollPosGlobal;
    private string searchString;
    private Texture2D headerDark;
    private Texture2D headerLight;
    private Texture2D header;
    private static int folderIndex;
    private static bool editFolder = false;
    public static bool isOpen = false;

    //scriptable object that stores global notes
    private GlobalNotes globalNoteDatabase;

    [MenuItem("Window/Project Notes")]
    public static void ShowWindow()
    {
        //EditorWindow.GetWindow(typeof(ProjectNotesEditor));
        EditorWindow.GetWindow(typeof(ProjectNotesEditor)).minSize = new Vector2(400, 320);
    }

    public static void NewFolder()
    {
        EditorWindow.GetWindow(typeof(ProjectNotesEditor)).minSize = new Vector2(400, 320);
        editFolder = true;
        folderIndex = 0;
    }

    void OnEnable()
    {
        globalNoteDatabase = GlobalListManagement.GetGlobalList();

        searchString = "";

        if (skin == null) // EditorGuiUtility.isProSkin
        {
            skinDark = EditorGUIUtility.Load("Assets/ProjectNotes/Resources/ProjectNotesStyle_Dark.guiskin") as GUISkin;
            skinLight = EditorGUIUtility.Load("Assets/ProjectNotes/Resources/ProjectNotesStyle_Light.guiskin") as GUISkin;
        }

        if (header == null)
        {
            headerDark = EditorGUIUtility.Load("Assets/ProjectNotes/Resources/DarkHeader.png") as Texture2D;
            headerLight = EditorGUIUtility.Load("Assets/ProjectNotes/Resources/Header.png") as Texture2D;
        }

        isOpen = true;
    }

    private void OnDisable()
    {
        isOpen = false;
    }

    void OnGUI()
    {
        //check for new notes
        if (ProjectNote.newNote)
        {
            //GetNoteList();
            ProjectNote.newNote = false;
        }

        if (EditorGUIUtility.isProSkin)
        {
            skin = skinDark;
            header = headerLight;
        }
        else
        {
            skin = skinLight;
            header = headerDark;
        }

        GUILayout.Label(header);


        if(editFolder)
        {
            DisplayFolders();
        }
        else
        {
            DisplayNotes();
            DisplayButtons();
        }

    }

    private void DisplayFolders()
    {
        GlobalNotes gn = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;

        scrollPosGlobal = EditorGUILayout.BeginScrollView(scrollPosGlobal, GUILayout.MinWidth(position.width));


        for (int i = 0; i < gn.noteFolders.Count; i++)
        {

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            gn.noteFolders[i] = EditorGUILayout.TextArea(gn.noteFolders[i], skin.customStyles[5], GUILayout.Width(300));
            if (GUILayout.Button("Remove", skin.button,GUILayout.Width(75)))
            {
                gn.noteFolders.RemoveAt(i);
                return;
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("New Folder", skin.button, GUILayout.Width(381)))
        {
            gn.noteFolders.Add("New Folder " + gn.noteFolders.Count.ToString());

        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();

        //if (GUILayout.Button("New Folder", skin.customStyles[6]))
        //{
        //    gn.noteFolders.Add("New Folder " + gn.noteFolders.Count.ToString());
        //} 
        if (GUILayout.Button("Close Folders", skin.button))
        {
            editFolder = !editFolder;
        }
        if (GUILayout.Button("Close Manager", skin.button))
        {
            this.Close();
        }

        EditorGUILayout.EndHorizontal();

    }

    private void DisplayNotes()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Folder : ", skin.label, GUILayout.Width(60));
        string[] folders = GlobalListManagement.GetFolderList();
        if (folderIndex >= folders.Length)
            folderIndex = 0;
        else if(folderIndex == folders.Length - 1)
        {
            editFolder = true;
            folderIndex = 0;
            return;
        }
        folderIndex = EditorGUILayout.Popup(folderIndex, folders, skin.customStyles[5], GUILayout.MaxWidth(300));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search : ", skin.label, GUILayout.Width(60));
        searchString = EditorGUILayout.TextField(searchString, skin.textField, GUILayout.MinWidth(275), GUILayout.MaxWidth(750));

        EditorGUILayout.EndHorizontal(); 

        //create list of global notes
        List<NoteInfo> tempGlobalNoteList = new List<NoteInfo>();
        foreach (NoteInfo note in globalNoteDatabase.globalNoteInfoList)
        {

            if (note == null || note.note == null)
                break;

            bool inSearch = (searchString == "" || CompareStrings(searchString, note.note.content) || CompareStrings(searchString, note.note.noteTitle));
            bool inFolder = (note.note.folderIndex == folderIndex);
            if (folderIndex == 0 || searchString != "")
                inFolder = true; //using "none" folder

            if (inSearch && inFolder)
                tempGlobalNoteList.Add(note);   

        }

        //display global notes list
        EditorGUILayout.BeginHorizontal();
        if (tempGlobalNoteList.Count > 0)
        {
            scrollPosGlobal = EditorGUILayout.BeginScrollView(scrollPosGlobal, GUILayout.MinWidth(position.width));

            foreach (NoteInfo info in tempGlobalNoteList)
            {

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                string buttonText;
                if (info.note.isOpen)
                    buttonText = "-";
                else
                    buttonText = "+";

                if (GUILayout.Button(buttonText, skin.customStyles[6], GUILayout.Width(20)))
                {
                    info.note.isOpen = !info.note.isOpen;
                }


                info.note.noteTitle = EditorGUILayout.TextArea(info.note.noteTitle, skin.customStyles[4]);

                if (info.note.isOpen && info.instanceID == 0)
                    EditorGUILayout.LabelField("No Location", skin.label);
                else if (info.note.isOpen)
                {
                    //ProjectNote pn = (ProjectNote)EditorUtility.InstanceIDToObject(info.instanceID);
                    ProjectNote pn = EditorUtility.InstanceIDToObject(info.instanceID) as ProjectNote;
                    if (pn != null)
                        EditorGUILayout.LabelField("On: " + pn.gameObject.name, skin.label);
                    else
                    {
                        if (Event.current.type != EventType.Layout && info.instanceID != 0)
                            globalNoteDatabase.globalNoteInfoList.Remove(info);
                        //EditorGUILayout.EndHorizontal();
                        //EditorGUILayout.EndScrollView();

                        //return;
                    }
                }
                else if (GUILayout.Button("Remove Note", skin.button, GUILayout.Width(125)))
                {
                    //DestroyImmediate(note);
                    globalNoteDatabase.globalNoteInfoList.Remove(info);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (info.note.isOpen)
                {
                    info.note.content = EditorGUILayout.TextArea(info.note.content, NoteBase.GetStyle(info.note.style, skin));

                    EditorGUILayout.BeginHorizontal();

                    info.note.style = (NoteStyle)EditorGUILayout.EnumPopup(info.note.style, skin.button);

                    if (info.note.folderIndex >= folders.Length)
                        info.note.folderIndex = 0;
                    else if (info.note.folderIndex == folders.Length - 1)
                    {
                        info.note.folderIndex = 0;
                        ProjectNotesEditor.NewFolder();
                    }
                    info.note.folderIndex = EditorGUILayout.Popup(info.note.folderIndex, folders, skin.button);

                    if (GUILayout.Button("Move", skin.button, GUILayout.MinWidth(75)))
                    {
                        MoveMenu(info);

                    }
                    if (info.instanceID != 0 && GUILayout.Button("Find In Scene", skin.button))
                    {
                        ProjectNote pn = (ProjectNote)EditorUtility.InstanceIDToObject(info.instanceID);
                        if(pn != null)
                            Selection.activeGameObject = pn.gameObject;
                    }
                    if (GUILayout.Button("Remove Note", skin.button, GUILayout.Width(125)))
                    {
                        if(info.instanceID != 0)
                        {
                            ProjectNote pn = (ProjectNote)EditorUtility.InstanceIDToObject(info.instanceID);
                            if(pn != null)
                                DestroyImmediate(pn);
                        }

                        globalNoteDatabase.globalNoteInfoList.Remove(info);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayButtons()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New Global Note", skin.button))
        {
            globalNoteDatabase.AddGlobalNote(System.Guid.NewGuid().ToString());
            ShowWindow();

        }
        if (GUILayout.Button("Edit Folders", skin.button))
        {
            editFolder = !editFolder;
        }
        if (GUILayout.Button("Toggle All Notes", skin.button))
        {
            if (globalNoteDatabase.globalNoteInfoList.Count == 0)
                return;
            
            bool open = !globalNoteDatabase.globalNoteInfoList[0].note.isOpen;

            foreach (NoteInfo info in globalNoteDatabase.globalNoteInfoList)
            {
                info.note.isOpen = open;
            }
        }
        if (GUILayout.Button("Close Manager", skin.button))
        {
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DisplayNoteButtons()
    {
        
    }


    private void MoveMenu(NoteInfo note)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Move Up"), false, ()=> MoveUp(note));
        menu.AddItem(new GUIContent("Move Down"), false, () => MoveDown(note));
        menu.AddItem(new GUIContent("Move to Top"), false, () => MoveToTop(note));
        menu.AddItem(new GUIContent("Move to Bottom"), false, () => MoveToBottom(note));
        menu.ShowAsContext();
    }

    public void MoveUp(NoteInfo note)
    {
        int currentIndex = globalNoteDatabase.globalNoteInfoList.IndexOf(note);
        if (currentIndex > 0)
            MoveNote(note, currentIndex - 1);
    }

    public void MoveDown(NoteInfo note)
    {
        int currentIndex = globalNoteDatabase.globalNoteInfoList.IndexOf(note);
        if (currentIndex < globalNoteDatabase.globalNoteInfoList.Count - 1)
            MoveNote(note, currentIndex + 1);
    }

    private void MoveToTop(NoteInfo note)
    {
        MoveNote(note, 0);
    }

    private void MoveToBottom(NoteInfo note)
    {
        MoveNote(note, globalNoteDatabase.globalNoteInfoList.Count - 1);
    }

    private void MoveNote(NoteInfo note, int location)
    {
        globalNoteDatabase.globalNoteInfoList.Remove(note);
        globalNoteDatabase.globalNoteInfoList.Insert(location, note);
    }
    ////Gets notes from scene
    //public static void GetNoteList()
    //{
    //    //if (Event.current.type != EventType.Repaint)
    //    //    return;

    //    noteList = GameObject.FindObjectsOfType<ProjectNote>();

    //    if (noteList.Length == 0)
    //    {
    //    }
    //    else
    //    {
    //        //store instance IDs
    //        noteIDs = new List<int>();
    //        foreach (ProjectNote note in noteList)
    //            noteIDs.Add(note.GetInstanceID());
    //    }
    //}

    //converts both strings to lower case to avoid case sensitive search
    bool CompareStrings(string search, string note)
    {
        string lowerSearch;
        string lowerNote;
        lowerSearch = search.ToLower();
        lowerNote = note.ToLower();

        return lowerNote.Contains(lowerSearch);
    }
}

public static class GlobalListManagement
{
#if UNITY_EDITOR
    //Finds data scriptable object
    public static GlobalNotes FindGlobalList()
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;
    }

    //creates new scriptable object
    public static GlobalNotes GetGlobalList()
    {
        if (FindGlobalList() != null)
            return FindGlobalList();

        Debug.LogError("Creating New Global List Data!!!");

        GlobalNotes data;
        data = ScriptableObject.CreateInstance<GlobalNotes>();
        string assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset");
        UnityEditor.AssetDatabase.CreateAsset(data, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        return data;
    }

    public static string[] GetFolderList()
    {
        GlobalNotes gn = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;
        List<string> tempFolderList = new List<string>(gn.noteFolders);
        tempFolderList.Add("-- Create New Folder --");
        tempFolderList.Insert(0,"No Folder");

        return tempFolderList.ToArray();
    }

#endif
}