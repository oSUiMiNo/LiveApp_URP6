using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProjectNote))]
public class ProjectNoteInspector : Editor
{

    GUISkin skinDark;
    GUISkin skinLight;
    GUISkin skin;
    GUIStyle noteStyle;
    private GlobalNotes gn;

    public override void OnInspectorGUI()
    {
        if (skin == null) // EditorGuiUtility.isProSkin
        {
            skinDark = EditorGUIUtility.Load("Assets/ProjectNotes/Resources/ProjectNotesStyle_Dark.guiskin") as GUISkin;
            skinLight = EditorGUIUtility.Load("Assets/ProjectNotes/Resources/ProjectNotesStyle_Light.guiskin") as GUISkin;
        }

        if (EditorGUIUtility.isProSkin)
            skin = skinDark;
        else
            skin = skinLight;

        ProjectNote noteScript = (ProjectNote)target;

        if (noteScript.noteInfo == null || noteScript.noteInfo.note == null)
            return;

        //Start of GUI
        EditorGUILayout.Space();
        noteScript.noteInfo.note.noteTitle = EditorGUILayout.TextArea(noteScript.noteInfo.note.noteTitle, skin.customStyles[4]);
        EditorGUILayout.Space();
        noteStyle = NoteBase.GetStyle(noteScript.noteInfo.note.style, skin);
        noteScript.noteInfo.note.content = EditorGUILayout.TextArea(noteScript.noteInfo.note.content, noteStyle);

        EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.LabelField("Style", skin.button, GUILayout.Width(40));

        noteScript.noteInfo.note.style = (NoteStyle)EditorGUILayout.EnumPopup(noteScript.noteInfo.note.style, skin.button);

        string[] folders = GlobalListManagement.GetFolderList();

        if (noteScript.noteInfo.note.folderIndex >= folders.Length)
            noteScript.noteInfo.note.folderIndex = 0;
        else if (noteScript.noteInfo.note.folderIndex == folders.Length - 1)
        {
            noteScript.noteInfo.note.folderIndex = 0;
            ProjectNotesEditor.NewFolder();
        }
        noteScript.noteInfo.note.folderIndex = EditorGUILayout.Popup(noteScript.noteInfo.note.folderIndex, folders, skin.button);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Search Notes", skin.button))
        {
            EditorApplication.ExecuteMenuItem("Window/Project Notes");
        }
        if (GUILayout.Button("Add Note", skin.button))
        {
            noteScript.gameObject.AddComponent<ProjectNote>();
        }
        if (GUILayout.Button("Remove", skin.button))
        {
            DestroyImmediate(noteScript);
        }

        EditorGUILayout.EndHorizontal();

        if(GUI.changed)
        {
            if (gn == null)
                gn = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;

            EditorUtility.SetDirty(gn);
        }


    }
}
