using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProjectNoteContextMenu {

    public class ContextMenuAdditions
    {
        [MenuItem("CONTEXT/ProjectNote/Convert To Global Note")]
        static void AddButtonStyle(MenuCommand command)
        {
            ProjectNote note = (ProjectNote)command.context;
            note.noteInfo.instanceID = 0;
            Editor.DestroyImmediate(note);
        }
    }
}
