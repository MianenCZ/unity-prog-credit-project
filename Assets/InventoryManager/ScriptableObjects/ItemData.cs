using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Provides default support for sorting items and specifing categories
/// </summary>
public enum ItemCategory
{
    Miscellaneous = 0,
    Weapons,
    Tools,
    Apparel,
    Materials,
    Consumables,
    Magic,
    Quest,
    //..
    //TODO: fill here for your convinience
}

/// <summary>
/// Defines multiselect for source of item.
/// </summary>
/// <remarks>this feaute is only for debugging purposes.</remarks>
[Flags]
public enum ItemSource
{
    None = 0,
    Crafting = 1 << 0,
    Loot = 1 << 1,
    Drop = 1 << 2,
    Quest = 1 << 3,
}




[CreateAssetMenu(fileName = "new_item", menuName = "Inventory Manager/Item")]
/// <summary>
/// See ItemDataEditor
/// </summary>
public class ItemData : ScriptableObject
{
    [Header("Basic info")]
    [HideInInspector]
    [Tooltip("Item Name in the native language. You can show this value to users in GUI.")]
    public string Name = "";
    [HideInInspector]
    [Tooltip("Globally unique name for internal purposes. Use snake case. You can show this value to users in debug GUI or command line.")]
    public string InternalName = "";
    [HideInInspector]
    [Tooltip("Optional description. Visible to user from GUI.")]
    [Multiline]
    public string Description = "";

    [Header("Inventory management")]
    [HideInInspector]
    [Tooltip("Defines if items can be stacked in storage.")]
    public bool Stackable = true;
    [HideInInspector]
    [Tooltip("Defines the size of the item stack in inventory. Has an effect only if Stackable is set to true. Value 1 has the same effect as Stackable set to false.")]
    public int StackSize = 100;
    [HideInInspector]
    [Tooltip("Defines if user can remove this item from storage.")]
    public bool Dropable = true;
    [HideInInspector]
    [Tooltip("Defines category for better sorting and search")]
    public ItemCategory Category = ItemCategory.Miscellaneous;
    [HideInInspector]
    [Tooltip("Mutiselect source of item (for debugging purposes)")]
    public ItemSource Source = ItemSource.None;


    [Header("3D object data")]
    [HideInInspector]
    [Tooltip("3D mesh for both inventory and in-game objects")]
    public Mesh Mesh = null;
    [HideInInspector]
    [Tooltip("3D material for both inventory and in-game objects")]
    public Material Material = null;


    [Header("Item icon")]
    [HideInInspector]
    [Tooltip("Scale for the game object when rendered as an inventory icon")]
    public Vector3 ItemIconScale = Vector3.one;
    [HideInInspector]
    [Tooltip("Offset for the game object when rendered as an inventory icon")]
    public Vector3 ItemIconOffset = Vector3.zero;
    [HideInInspector]
    [Tooltip("Rotation for the game object when rendered as an inventory icon")]
    public Vector3 ItemIconRotation = Vector3.zero;

    [Header("Item ingame")]
    [HideInInspector]
    [Tooltip("Scale for the game object when rendered as an in-game object")]
    public Vector3 ItemIngameScale = Vector3.one;
    [HideInInspector]
    [Tooltip("Offset for the game object when rendered as an in-game object")]
    public Vector3 ItemIngameOffset = Vector3.zero;

    [Header("Item editor")]
    [HideInInspector]
    [Tooltip("Offset for the game object when rendered as editor view")]
    public Vector3 EditorIconScale = Vector3.one;
    [HideInInspector]
    [Tooltip("Offset for the game object when rendered as editor view")]
    public Vector3 EditorIconOffset = Vector3.one;
    [HideInInspector]
    [Tooltip("Rotation for the game object when rendered as editor view")]
    public Vector3 EditorIconRotation = Vector3.zero;

    private static string ResourcePath => @"Assets\Resources\Items";



#if UNITY_EDITOR

    /// <summary>
    /// Returns all scriptable objects of type <see cref="ItemData"/>
    /// </summary>
    /// <returns>Loaded scriptable objects</returns>
    public static List<ItemData> GetAllItems()
    {
        List<ItemData> allItems = new List<ItemData>();
        foreach (var id in AssetDatabase.FindAssets("t:ItemData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            allItems.Add(sourceAsset);
        }
        return allItems;
    }

    private static Dictionary<string, ItemData> _getAllItemsCache = null;
    /// <summary>
    /// Returns all scriptable objects of type <see cref="ItemData"/> as dictionary indexed by <see cref="InternalName"/>. 
    /// Load is executed only once, on first call.
    /// </summary>
    /// <returns>Loaded scriptable objects</returns>
    public static Dictionary<string, ItemData> GetAllItemsCached()
    {
        if (_getAllItemsCache == null)
            _getAllItemsCache = GetAllItems().ToDictionary(x => x.InternalName);
        return _getAllItemsCache;
    }

    public static Dictionary<string, ItemData> RefreshItemCache()
    {
        _getAllItemsCache = GetAllItems().ToDictionary(x => x.InternalName);
        return _getAllItemsCache;
    }

    [MenuItem("Tools/Inventory manager/Create new item")]
    public static void CreateNewItem()
    {
        var path = Path.Combine(ResourcePath, "item.asset");
        var data = CreateInstance<ItemData>();
        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(data, path);
    }

    [MenuItem("Assets/Inventory manager/Convert asset to item")]
    public static void ConvertToItem()
    {
        var selection = Selection.assetGUIDs;
        foreach (var assetGuid in selection)
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<GameObject>(clickedPath);

            if (sourceAsset == null)
            {
                Debug.LogError($"Path '{clickedPath}' does define '{typeof(GameObject)}'.");
                continue;
            }

            var meshFilter = sourceAsset.GetComponent<MeshFilter>();
            var mesh = meshFilter?.sharedMesh;
            var meshRenderer = sourceAsset.GetComponent<MeshRenderer>();
            var material = meshRenderer?.sharedMaterial;

            if (material == null)
            {
                Debug.LogError($"Asset on path '{clickedPath}' does not contains '{typeof(Material)}'.");
                continue;
            }
            if (mesh == null)
            {
                Debug.LogError($"Asset on path '{clickedPath}' does not contains '{typeof(Mesh)}'.");
                continue;
            }

            var data = CreateInstance<ItemData>();
            data.Name = sourceAsset.name.Replace('_', ' ');
            data.InternalName = sourceAsset.name.ToLower();
            data.Mesh = mesh;
            data.Material = material;

            var path = Path.Combine(ResourcePath, $"{data.InternalName}.asset");
            var testPath = AssetDatabase.AssetPathToGUID(path);
            Debug.Log(testPath);
            if (!string.IsNullOrEmpty(testPath))
            {
                Debug.LogError($"Asset on path '{path}' already exists.");
                continue;
            }

            Directory.CreateDirectory(ResourcePath);
            if (selection.Length > 1)
                AssetDatabase.CreateAsset(data, path);
            else
                ProjectWindowUtil.CreateAsset(data, path);
        }
    }

#endif
}
