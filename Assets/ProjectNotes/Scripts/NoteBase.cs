using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteBase
{
    public string noteTitle = "Note Title (Edit Me!)";
    public string content = "Put Your Note Here";
    //[SerializeField]
    public NoteStyle style = NoteStyle.blue;
    public bool isOpen = true;
    public int folderIndex; //used to sort notes into folders


    public static GUIStyle GetStyle(NoteStyle style, GUISkin skin)
    {
        GUIStyle _style;

        _style = skin.GetStyle(style.ToString());

        if (_style == null)
            _style = skin.GetStyle("blue");

        return _style;
    }
}


