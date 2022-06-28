using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventoryManagerSettings : ScriptableObject
{
    [Header("Inventory manager settings")]
    [Tooltip("Active settings. ONLY ONE SETTINGS CAN BE ACTIVE!")]
    public bool Active = true;

    [Header("Preview windows settings")]
    [Tooltip("Offset of camera inside renderer")]
    public Color BackgroundColor = Color.black;
    [Tooltip("Offset of camera inside renderer")]
    public Vector3 CameraPosition = new Vector3(0, 0, -2.5f);
    [Tooltip("Camera clear flags")]
    public CameraClearFlags CameraClearFlags = CameraClearFlags.SolidColor;

    [Space]
    [Tooltip("Size of image for list preview ")]
    public float ListPreviewImageSize = 75;
    [Tooltip("Size of scape between image for lists preview")]
    public float ListPreviewSpaceSize = 10f;
    [Tooltip("Size of image for single item preview")]
    public float DetailPreviewMaxImageSize = 300;

    [Space]
    [Tooltip("Offset of label for ItemAmountListing and LootEntry")]
    public Vector2 LabelOffset = new Vector2(5, 2);
    [Tooltip("Font size of label for ItemAmountListing and LootEntry")]
    public int LabelFontSize = 13;
    [Tooltip("Font style of label for ItemAmountListing and LootEntry")]
    public FontStyle LabelFontStyle = FontStyle.Normal;
    [Tooltip("Foreground color of label for ItemAmountListing and LootEntry")]
    public Color LableColor = Color.white;

    private static string ResourcePath => @"Assets\Settings";

#if UNITY_EDITOR
    public static List<InventoryManagerSettings> GetAllSettings()
    {
        List<InventoryManagerSettings> allItems = new List<InventoryManagerSettings>();
        foreach (var id in AssetDatabase.FindAssets("t:InventoryManagerSettings"))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<InventoryManagerSettings>(path);
            allItems.Add(sourceAsset);
        }
        return allItems;
    }

    public static InventoryManagerSettings GetActiveSettings()
    {
        var res = GetAllSettings().Where(x => x.Active).ToList();
        if (res.Count == 0)
        {
            Debug.Log("No active Inventory manager settins found. Fallback to default settings.");
            return new InventoryManagerSettings();
        }
        if (res.Count > 1)
        {
            Debug.Log("More then one Inventory manager settins is active. Fallback to default settings.");
            return new InventoryManagerSettings();
        }
        return res[0];
    }


    [MenuItem("Tools/Inventory manager/Create new settings")]
    public static void CreateNewItem()
    {
        var path = Path.Combine(ResourcePath, "inventory_manager_settings.asset");
        var data = CreateInstance<InventoryManagerSettings>();
        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(data, path);
    }

#endif
}