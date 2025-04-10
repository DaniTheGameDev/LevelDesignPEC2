using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string prefabName;
    public Vector3 position;
}

public class SokobanEditorSetup : MonoBehaviour
{
    [HideInInspector] public Vector2 position = new Vector2(20, 20);
    [HideInInspector] public float width = 300;
    [HideInInspector] public float height = 100;

    private string prefabsPath = "Prefabs";
    
    public Sprite cellSprite;
    private Color cellColor = Color.white;
    private int cellWidth = 13;
    private int cellHeight = 13;
    private float cellSize = 1;

    [Header("Parent object of the map")]
    public Transform map;
    
    [Header("Level Data")]
    public LevelDataScriptableObject levelDataObject;

    [HideInInspector] public Transform[] prefabs;
    [HideInInspector] public string[] prefabsNames;
    [HideInInspector] public int index;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ClearMap()
    {
        GetClear(map);
    }

    private void GetClear(Transform tr)
    {
        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < tr.childCount; i++)
        {
            children.Add(tr.GetChild(i).gameObject);
        }

        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
        }
    }

    public void Create()
    {
        GetClear(transform);

        Transform t = new GameObject().transform;
        t.gameObject.tag = "EditorOnly";
        t.gameObject.AddComponent<SpriteRenderer>().sprite = cellSprite;
        t.gameObject.GetComponent<SpriteRenderer>().color = cellColor;
        t.gameObject.AddComponent<BoxCollider2D>();

        float posX = -cellSize * cellWidth / 2 - cellSize / 2;
        float posY = cellSize * cellHeight / 2 + cellSize / 2;
        float Xreset = posX;

        int z = 0;
        for (int y = 0; y < cellHeight; y++)
        {
            posY -= cellSize;
            for (int x = 0; x < cellWidth; x++)
            {
                posX += cellSize;
                Transform tr = Instantiate(t);
                tr.SetParent(transform);
                tr.localScale = Vector3.one;
                tr.position = new Vector2(posX, posY);
                tr.name = "Cell_" + z;
                z++;
            }
            posX = Xreset;
        }

        DestroyImmediate(t.gameObject);
    }
    
    public void SetPrefab(GameObject obj)
    {
        if(prefabs.Length == 0) return;
        Transform clone = Instantiate(prefabs[index], obj.transform.position - Vector3.forward * 0.05f, Quaternion.identity) as Transform;
        clone.gameObject.name = prefabs[index].name;
        clone.parent = map;
    }

    public void SaveLevelToScriptableObject(LevelDataScriptableObject levelDataObject)
    {
        levelDataObject.levels.Clear();

        for (int i = 0; i < map.childCount; i++)
        {
            Transform child = map.GetChild(i);
            levelDataObject.levels.Add(new LevelData
            {
                prefabName = child.name,
                position = child.position
            });
        }
        UnityEditor.EditorUtility.SetDirty(levelDataObject);
        Debug.Log("¡Level saved to ScriptableObject!");
    }

    public void LoadLevelFromScriptableObject(LevelDataScriptableObject levelDataObject)
    {
        ClearMap();
        foreach (LevelData data in levelDataObject.levels)
        {
            foreach (Transform prefab in prefabs)
            {
                if (prefab.name == data.prefabName)
                {
                    Transform instance = Instantiate(prefab, data.position, Quaternion.identity);
                    instance.parent = map;
                    instance.name = prefab.name;
                    break;
                }
            }
        }

        Debug.Log("Level loaded from ScriptableObject!");
    }

    public void LoadResources()
    {
        prefabs = Resources.LoadAll<Transform>(prefabsPath);
        prefabsNames = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabsNames[i] = prefabs[i].name;
        }
        index = 0;
    }
}