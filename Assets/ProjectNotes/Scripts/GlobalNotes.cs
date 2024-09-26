using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GlobalNotes : ScriptableObject
{

    [SerializeField]
    public List<NoteInfo> globalNoteInfoList = new List<NoteInfo>();

    public NoteInfo AddGlobalNote(string GUID, ProjectNote pn = null)
    {
        NoteInfo newNote = new NoteInfo(GUID, pn);
        globalNoteInfoList.Add(newNote);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        return newNote;
    }

    [SerializeField]
    public List<string> noteFolders = new List<string>();
}

[System.Serializable]
public class NoteInfo
{
    public NoteInfo(string GUID, ProjectNote pn = null)
    {
        if(pn != null)
        {
            isGlobal = false;
            //instanceID = pn.GetInstanceID();
        }
        else
        {
            isGlobal = true;
            pn = null;
        }

        this.GUID = GUID;
    }

    public NoteBase note = new NoteBase();
    public bool isGlobal;
    public int instanceID;
    public string GUID;
}

