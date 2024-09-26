/// version 1.1


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace WorkingSet
{

    class WorkingSetWindow : EditorWindow
    {
        public static string plugins_path = @"Assets\EditorWorkingSet\Editor\";

        [MenuItem("Window/Working Set Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(WorkingSetWindow));
        }


        void OnSelectionChange()
        {
            if (working_set_data != null)
            {
                working_set_data.ResetAllInfo();
                Repaint();
            }
        }
        void OnHierarchyChange()
        {
            if (working_set_data != null)
            {
                working_set_data.ResetAllInfo();
                Repaint();
            }
        }

        void OnProjectChange()
        {
            if (working_set_data != null)
            {
                working_set_data.ResetAllInfo();
                Repaint();
            }
        }

        private class FavoritesTabAssetPostprocessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (deletedAssets.Length > 0 || movedAssets.Length > 0)
                {
                    WorkingSetWindow.LoadAll();
                }
            }
        }

        public static void LoadAll()
        {
            if (working_set_data == null) return;

            working_set_data.ResetAllInfo();
            EditorUtility.SetDirty(working_set_data);
        }



        Vector2 scrol_pos;
        int selected_item_index = -1;
        int drag_item_index = -1;
        Vector2 mouse_down_pos;
        float last_click_time;

        bool drag_from_self;
        Texture2D selected_background;

        static WorkingSetData working_set_data;

        void Init()
        {
#if UNITY_5_3
            this.titleContent = new GUIContent("Working Set");
#else
            this.title = "Working Set";
#endif
            if (working_set_data == null)
            {
                string path = WorkingSetWindow.plugins_path + "working_set_data.asset";
                WorkingSetData data = AssetDatabase.LoadAssetAtPath(path, typeof(WorkingSetData)) as WorkingSetData;
                if (data == null)
                {
                    data = WorkingSetData.CreateInstance<WorkingSetData>();
                    AssetDatabase.CreateAsset(data, path);
                }
                data.ResetAllInfo();
                working_set_data = data;
            }
            if (selected_background == null)
            {
                selected_background = new Texture2D(10, 10);
                Color color = new Color(0.2f, 0.4f, 0.6f);
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        selected_background.SetPixel(i, j, color);
                    }
                }
                selected_background.Apply();
            }

        }
        void OnGUI()
        {
            Init();

            DrawHead();

            Event current = Event.current;

            bool repaint = false;
            bool list_modified = false;

            if (current.type == EventType.MouseDrag && drag_item_index >= 0 && drag_item_index < working_set_data.datas.Count)
            {
                WorkingSetData.Item it = working_set_data.datas[drag_item_index];

                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new Object[1] { it.obj };
                DragAndDrop.StartDrag(it.name);

                drag_from_self = true;
                drag_item_index = -1;
            }

            EditorGUIUtility.SetIconSize(new Vector2(24f, 24f));
            scrol_pos = EditorGUILayout.BeginScrollView(scrol_pos);

            if (working_set_data.datas.Count == 0)
            {
                GUILayout.Label("Drag some thing here");
                if (Event.current.type == EventType.DragPerform)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        working_set_data.AddLast(DragAndDrop.objectReferences[i]);
                    }

                    DragAndDrop.AcceptDrag();
                    Event.current.Use();

                    repaint = true;

                    drag_from_self = false;
                }
            }
            else
            {
                for (int index = 0; index < working_set_data.datas.Count; index++)
                {
                    WorkingSetData.Item item = working_set_data.datas[index];
                    //if (item.object_type == WorkingSetData.ObjectType.GameObject && Application.isPlaying) continue;
                    if (item.obj == null) continue;

                    GUIStyle style = null;
                    if (Selection.activeObject == item.obj)
                    {
                        style = new GUIStyle();
                        style.fontStyle = FontStyle.Bold;
                        style.normal.background = selected_background;
                    }
                    if (item.object_type == WorkingSetData.ObjectType.GameObject)
                    {
                        if (style == null) style = new GUIStyle();
                        style.normal.textColor = Color.yellow;
                    }
                    else if (item.object_type == WorkingSetData.ObjectType.Prefab)
                    {
                        if (style == null) style = new GUIStyle();
                        style.normal.textColor = Color.green;
                    }

                    if (style != null) GUILayout.Label(item.gui_content, style);
                    else GUILayout.Label(item.gui_content);

                    Rect rt = GUILayoutUtility.GetLastRect();

                    if (current.type == EventType.MouseDown)
                    {
                        if (rt.Contains(current.mousePosition))
                        {
                            selected_item_index = index;
                            mouse_down_pos = current.mousePosition;
                            drag_item_index = index;
                        }
                    }
                    if (current.type == EventType.MouseUp)
                    {
                        drag_item_index = -1;
                        if (current.button == 0 && index == selected_item_index)
                        {
                            if (rt.Contains(current.mousePosition))
                            {
                                bool double_click = Time.realtimeSinceStartup - last_click_time < 0.3f;
                                HilightObject(item, double_click);
                                last_click_time = Time.realtimeSinceStartup;
                                current.Use();
                            }
                        }
                        if (current.button == 2 && index == selected_item_index)
                        {
                            if (rt.Contains(current.mousePosition))
                            {
                                working_set_data.RemoveAt(index);

                                DragAndDrop.AcceptDrag();
                                Event.current.Use();

                                repaint = true;
                                break;
                            }
                        }
                    }
                    if (Event.current.type == EventType.DragPerform)
                    {
                        int insert_index = GetInsertIndex(rt, mouse_down_pos, current.mousePosition,  index, working_set_data.datas.Count);
                        if (insert_index >= 0)
                        {

                            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                            {
                                working_set_data.InsertAt(insert_index, DragAndDrop.objectReferences[i]);
                            }
                            list_modified = true;

                            DragAndDrop.AcceptDrag();
                            Event.current.Use();

                            repaint = true;
                            break;
                        }
                        drag_from_self = false;
                    }
                    //EditorGUILayout.EndHorizontal();
                }

            }

            EditorGUILayout.EndScrollView();
            GUILayout.Space(20);


            if (current.type == EventType.DragUpdated)
            {
                if (drag_from_self)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
            }
            if (selected_item_index >= 0 && selected_item_index < working_set_data.datas.Count)
            {
                WorkingSetData.Item it = working_set_data.datas[selected_item_index];
                string path = it.path;
                string asset_head = "Assets";

                GUIStyle style = new GUIStyle();
                //style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;

                if (path.StartsWith(asset_head)) path = path.Substring(asset_head.Length, path.Length - asset_head.Length);
                if (path == "") path = it.name;

                if (it.object_type != WorkingSetData.ObjectType.Other) GUI.Label(detial_rc, it.object_type.ToString() + " : " + path, style);
                else GUI.Label(detial_rc, path, style);
            }

            if (list_modified)
            {
                selected_item_index = -1;
                working_set_data.ResetAllInfo();
                EditorUtility.SetDirty(working_set_data);
            }

            if (repaint || list_modified)
            {
                Repaint();
            }
        }

        Rect detial_rc;
        void DrawHead()
        {
            GUILayout.Space(10f);
            //GUILayout.BeginHorizontal();
            GUILayout.Label("");
            detial_rc = GUILayoutUtility.GetLastRect();

            Rect rt = detial_rc;
            rt.xMin = rt.xMax - 40f;
            if (GUI.Button(rt, "help"))
            {
                WSHelpWindow.GetWindow<WSHelpWindow>();
            }
            //GUILayout.EndHorizontal();
        }

        int GetInsertIndex(Rect rt, Vector2 mouse_start_pos, Vector2 mouse_current_pos,  int index, int count)
        {
            Rect origin_rt = rt;
            rt.yMin -= 5;
            rt.yMax += 5;
            int insert_index = -1;
            if (mouse_current_pos.x >= rt.xMin && mouse_current_pos.x <= rt.xMax)
            {
                if (index == count - 1)
                {
                    if (mouse_current_pos.y >= rt.yMin)
                    {
                        insert_index = index;
                        if (mouse_current_pos.y > rt.center.y) insert_index++;
                    }
                }
                else
                {
                    if (mouse_current_pos.y >= rt.yMin && mouse_current_pos.y <= rt.yMax)
                    {
                        insert_index = index;
                        if (mouse_current_pos.y > rt.center.y) insert_index++;
                    }
                }
            }

            if (insert_index >= 0 && origin_rt.Contains(mouse_start_pos))
            {
                Vector2 offset = mouse_current_pos - mouse_start_pos;
                if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
                {
                    if (offset.x > rt.width * 0.2f) insert_index = count;
                    else if (offset.x < -rt.width * 0.2f) insert_index = 0;
                }
            }

            return insert_index;
        }

        static string ExtRemoveEnd(string text, string sub)
        {
            int i = text.LastIndexOf(sub);
            if (i >= 0)
            {
                text = text.Substring(0, text.Length - (text.Length - i));
            }
            return text;
        }

        void HilightObject(WorkingSetData.Item item, bool double_click)
        {
            if (Event.current.shift)
            {
                if (item.path == "")
                {
                    //EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                    SceneView.FrameLastActiveSceneView();
                }
                else EditorApplication.ExecuteMenuItem("Window/Project");
            }
            PathParser paser = PathParser.Parse(item.path);
            bool is_scene_object = paser.FileName == "";
            bool is_folder=false;
            string abusolute_path = "";
            if (!is_scene_object)
            {
                abusolute_path = ExtRemoveEnd(Application.dataPath, "Assets") + item.path;
                is_folder = System.IO.Directory.Exists(abusolute_path);
            }


            if (double_click && Event.current.control && !is_scene_object)
            {
                Selection.activeObject = item.obj;
                EditorGUIUtility.PingObject(item.obj);
                EditorApplication.ExecuteMenuItem("Assets/Show in Explorer");
                return;
            }

            Object ping_object = item.obj;
            if (double_click && is_folder) // is folder
            {

                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(abusolute_path);
                System.IO.FileInfo[] files = info.GetFiles();
                bool find_file = false;
                for (int i = 0; i < files.Length; i++)
                {
                    string name = files[i].Name;
                    if (name.EndsWith(".meta")) continue;
                    string path = PathParser.CombinePaths(paser.FullPath, name);
                    ping_object = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                    find_file = true;
                }
                if (!find_file)
                {
                    System.IO.DirectoryInfo[] dirs = info.GetDirectories();
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        string name = dirs[i].Name;
                        string path = PathParser.CombinePaths(paser.FullPath, name);
                        ping_object = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));
                        find_file = true;
                    }
                }


                Selection.activeObject = item.obj;
                EditorGUIUtility.PingObject(ping_object);
            }
            else
            {
                Selection.activeObject = ping_object;
                EditorGUIUtility.PingObject(ping_object);
                if (double_click && !is_scene_object)
                {
                    AssetDatabase.OpenAsset(ping_object);
                }
            }
        }

    }

    class WSHelpWindow : EditorWindow
    {
        public WSHelpWindow()
        {
#if UNITY_5_3
            this.titleContent = new GUIContent("Help");
#else
            this.title = "Working Set";
#endif
        }
        void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.richText = true;

            GUILayout.Label("Working Set Operations:");

            GUILayout.Label("<color=yellow>drag object in to window </color>: add item", style);
            GUILayout.Label("<color=yellow>drag item up or down </color>: move item up or down", style);
            GUILayout.Label("<color=yellow>drag item left </color>: move item to top", style);
            GUILayout.Label("<color=yellow>drag item right </color>: move item to bottom", style);
            GUILayout.Label("<color=yellow>middle click</color>: remove item", style);

            GUILayout.Space(20f);

            GUILayout.Label("<color=yellow>click</color>: select item", style);
            GUILayout.Label("<color=yellow>shift + click</color>: force hilight in project view or scene view", style);

            GUILayout.Space(20f);

            GUILayout.Label("<color=yellow>double click</color>: open item", style);
            GUILayout.Label("<color=yellow>ctrl + double click item</color>: explore item in folder", style);

        }
    }
}