using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SokobanEditorSetup))]
public class SokobanEditor : Editor
{
    private LevelDataScriptableObject levelDataObject;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SokobanEditorSetup editorSetup = (SokobanEditorSetup)target;
        GUILayout.Label("Map Editor Controls", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create / Update Grid"))
        {
            editorSetup.Create();
        }
        if (GUILayout.Button("Clear Map"))
        {
            editorSetup.ClearMap();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Level Save/Load Controls", EditorStyles.boldLabel);
        if (GUILayout.Button("Save Level to ScriptableObject"))
        {
            editorSetup.SaveLevelToScriptableObject(editorSetup.levelDataObject);
        }

        if (GUILayout.Button("Load Level from ScriptableObject"))
        {
            editorSetup.LoadLevelFromScriptableObject(editorSetup.levelDataObject);
        }
    }

    void OnSceneGUI()
    {
        SokobanEditorSetup editorSetup = (SokobanEditorSetup)target;
        
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        
        if (Event.current.button == 0 && 
           (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
        {
            HandleMouseInput(editorSetup);
        }
        
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(editorSetup.position.x, editorSetup.position.y, editorSetup.width, editorSetup.height), EditorStyles.helpBox);

        if (GUILayout.Button("Load / Refresh Prefabs"))
        {
            editorSetup.LoadResources();
        }

        GUILayout.Label("Shortcut: Place selected prefab with Left Mouse.");
        GUILayout.Label("Remove objects with Shift + Left Mouse.");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Selected Prefab: ");
        editorSetup.index = EditorGUILayout.Popup(editorSetup.index, editorSetup.prefabsNames);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
        Handles.EndGUI();
    }
    
    private void HandleMouseInput(SokobanEditorSetup editorSetup)
    {
        Vector2 mousePos = Event.current.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(
            SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(new Vector2(
                mousePos.x, SceneView.currentDrawingSceneView.camera.pixelHeight - mousePos.y
            )),
            Vector2.zero
        );

        if (hit.collider != null)
        {
            if (!Event.current.shift)
            {
                if (hit.collider.name.CompareTo(editorSetup.prefabsNames[editorSetup.index]) != 0)
                {
                    editorSetup.SetPrefab(hit.collider.gameObject);
                }
            }
            else
            {
                if (hit.collider.tag.CompareTo("EditorOnly") != 0)
                {
                    DestroyImmediate(hit.collider.gameObject);
                }
            }
        }
    }
}