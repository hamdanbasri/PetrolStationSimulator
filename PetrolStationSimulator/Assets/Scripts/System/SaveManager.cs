using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Wrapper classes marked Serializable so Unity's JsonUtility can process lists accurately
[Serializable]
public class PlacedObjectSaveEntry
{
    public string itemTypeID;
    public Vector3 position;
    public Vector3 rotationData; // Storing as Euler angles for readability and precision in JSON
}

[Serializable]
public class GameSaveDataWrapper
{
    public List<PlacedObjectSaveEntry> placedObjects = new List<PlacedObjectSaveEntry>();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Prefab Database Reference")]
    [Tooltip("Assign all available buyable/placeable item prefabs here. Ensure their 'itemTypeID' match their configurations.")]
    public List<GameObject> masterPrefabDatabase = new List<GameObject>();

    // Central registry tracking active elements in the scene
    private List<PlacedObjectData> activePlacedObjects = new List<PlacedObjectData>();
    private string saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: preserves state transitions across menu reloads
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Establish safe persistent storage path target on Windows (%userprofile%/AppData/LocalLow/...)
        saveFilePath = Path.Combine(Application.persistentDataPath, "gas_station_save.json");
    }

    void Start()
    {
        // Automatically attempt to load existing layouts when scene initializes
        LoadGame();
    }

    // Handles the requested condition to automatically save layout elements when a user exits the application
    void OnApplicationQuit()
    {
        SaveGame();
    }

    #region Registry Controls
    public void RegisterObject(PlacedObjectData obj)
    {
        if (!activePlacedObjects.Contains(obj))
        {
            activePlacedObjects.Add(obj);
        }
    }

    public void UnregisterObject(PlacedObjectData obj)
    {
        if (activePlacedObjects.Contains(obj))
        {
            activePlacedObjects.Remove(obj);
        }
    }
    #endregion

    #region Save & Load Systems
    public void SaveGame()
    {
        GameSaveDataWrapper saveData = new GameSaveDataWrapper();

        foreach (PlacedObjectData objData in activePlacedObjects)
        {
            if (objData == null) continue;

            if (string.IsNullOrEmpty(objData.itemTypeID))
            {
                Debug.LogWarning($"[SaveManager] Object named '{objData.gameObject.name}' doesn't have an itemTypeID configured! Skipping record.", objData);
                continue;
            }

            PlacedObjectSaveEntry entry = new PlacedObjectSaveEntry
            {
                itemTypeID = objData.itemTypeID,
                position = objData.transform.position,
                rotationData = objData.transform.eulerAngles
            };

            saveData.placedObjects.Add(entry);
        }

        try
        {
            // Convert objects structured collection to clean JSON formatting text representation
            string jsonString = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, jsonString);
            Debug.Log($"[SaveManager] Layout safely written to storage pathway successfully: {saveFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveManager] Error occurred during serialization execution routines: {ex.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("[SaveManager] No previous game save layout file discovered. Starting clear workspace layout.");
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(saveFilePath);
            GameSaveDataWrapper loadedData = JsonUtility.FromJson<GameSaveDataWrapper>(jsonString);

            if (loadedData == null || loadedData.placedObjects == null) return;

            // Clear any physical objects existing right now to avoid duplication when invoking a historical sequence
            WipeActiveSceneObjects();

            // Reconstruct the saved petrol station structure assembly loop
            foreach (PlacedObjectSaveEntry entry in loadedData.placedObjects)
            {
                GameObject prefabToSpawn = FindPrefabByID(entry.itemTypeID);

                if (prefabToSpawn != null)
                {
                    GameObject spawnedObj = Instantiate(prefabToSpawn, entry.position, Quaternion.Euler(entry.rotationData));
                    
                    // Assign layers manually to align with your project's validation masking rules
                    if (GridSystem.Instance != null)
                    {
                        int layerIndex = Mathf.RoundToInt(Mathf.Log(GridSystem.Instance.placedObjectsLayer.value, 2));
                        SetLayerRecursively(spawnedObj, layerIndex);

                        // Ensure references match up dynamically for edit execution routines 
                        PlacedObjectData dataComponent = spawnedObj.GetComponent<PlacedObjectData>();
                        if (dataComponent != null)
                        {
                            dataComponent.originalPrefab = prefabToSpawn;
                        }
                    }
                }
                else
                {
                    Debug.LogError($"[SaveManager] Could not restore object structure! '{entry.itemTypeID}' missing inside Master Database array mapping inventory.");
                }
            }
            Debug.Log("[SaveManager] Scene serialization elements deployed back into active frame state arrays.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveManager] System execution faulted processing loading serialization text lines: {ex.Message}");
        }
    }

    // Developer function optimized for iteration workflows and QA testing environments
    public void ResetSaveData()
    {
        Debug.Log("[SaveManager] Initializing hard verification flush of active runtime properties and binary data files...");
        
        // 1. Physically destroy all items currently built on your station map layout structure canvas
        WipeActiveSceneObjects();

        // 2. Erase the persistent JSON save path securely from disk
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("[SaveManager] Target text configurations file tracking system assets successfully destroyed.");
        }
    }

    private void WipeActiveSceneObjects()
    {
        // Loop backwards safely deleting components without breaking indices arrays sizes during destruction cycle iterations
        for (int i = activePlacedObjects.Count - 1; i >= 0; i--)
        {
            if (activePlacedObjects[i] != null)
            {
                Destroy(activePlacedObjects[i].gameObject);
            }
        }
        activePlacedObjects.Clear();
    }

    private GameObject FindPrefabByID(string id)
    {
        return masterPrefabDatabase.Find(prefab =>
        {
            PlacedObjectData data = prefab.GetComponent<PlacedObjectData>();
            return data != null && data.itemTypeID == id;
        });
    }

    private void SetLayerRecursively(GameObject target, int layerIndex)
    {
        target.layer = layerIndex;
        foreach (Transform child in target.transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }
    #endregion
}