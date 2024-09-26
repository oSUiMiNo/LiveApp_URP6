using UnityEngine;
using System.Collections;

[System.Serializable]
public class ProjectNote : MonoBehaviour
{
    public NoteInfo noteInfo
    {
        get
        {
#if UNITY_EDITOR
            if (gn == null)
                gn = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;
            
            foreach (NoteInfo n in gn.globalNoteInfoList)
            {
                if (n.GUID == this.GUID)
                    return n;
            }

#endif

            if (gn != null)
            {
                GUID = System.Guid.NewGuid().ToString();
                return gn.AddGlobalNote(GUID, this);
            }
            else
                return null;
        }
    }

    private GlobalNotes gn;
    public static bool newNote = true;
    [SerializeField]
    [HideInInspector]
    private string GUID;

    private void Reset()
    {
#if Unity_Editor
        newNote = true;
        if(gn == null)
            gn = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;

        gn.AddGlobalNote(this);
#endif
    }

    //private void OnDisable()
    //{
    //    Debug.Log("Disabling");
    //}

    //private void OnDestroy()
    //{
    //    Debug.Log("Removing");
    //}

#if UNITY_EDITOR

    private void OnGUI()
    {

        if (GUI.changed)
        {
            if (gn == null)
                gn = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/ProjectNotes/Resources/GlobalNoteDatabase.asset", typeof(GlobalNotes)) as GlobalNotes;

            UnityEditor.EditorUtility.SetDirty(gn);
            //Debug.Log("Something Changed");
        }
    }
#endif
}

[System.Serializable]
public enum NoteStyle
{
    blue,
    green,
    pink,
    yellow
}
