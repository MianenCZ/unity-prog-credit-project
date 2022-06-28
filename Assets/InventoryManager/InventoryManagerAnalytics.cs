using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class InventoryManagerAnalytics
{
    private class AnalyzeItemDataDto
    {
        public string Path { get; set; }
        public ItemData ItemData { get; set; }

        public AnalyzeItemDataDto(ItemData itemData, string path)
        {
            ItemData = itemData;
            Path = path;
        }
    }

#if UNITY_EDITOR

    [MenuItem("Tools/Inventory manager/Analytics/Run full analysis")]
    public static void AnalyzeAssets()
    {
        AnalyzeAssetsNamingConvention();
        AnalyzeAssetsDefinitions();
        AnalyzeAssetsObtainability();
    }

    [MenuItem("Tools/Inventory manager/Analytics/Analyze item naming convention")]
    public static void AnalyzeAssetsNamingConvention()
    {
        var allItems = getAssets();


        var internalNamesColisions = allItems.GroupBy(x => x.ItemData.InternalName)
                                             .Where(x => x.Count() > 1);
        var namesColisions = allItems.GroupBy(x => x.ItemData.Name)
                                     .Where(x => x.Count() > 1);

        Debug.ClearDeveloperConsole();
        foreach (var collision in internalNamesColisions)
        {
            Debug.LogErrorFormat(
                "Internal name '{0}' is in conflict within following files: {1}.",
                collision.Key,
                string.Join("; ", collision.Select(x => x.Path))
                );
        }
        foreach (var collision in namesColisions)
        {
            Debug.LogErrorFormat(
                "Name '{0}' is in conflict within following files: {1}.",
                collision.Key,
                string.Join("; ", collision.Select(x => x.Path))
                );
        }
        foreach (var item in allItems)
        {
            var snakeCase = item.ItemData.InternalName.Trim().Replace(' ', '_').ToLowerInvariant();
            if (snakeCase != item.ItemData.InternalName)
            {
                Debug.LogWarning($"Internal name '{item.ItemData.InternalName}' don't fit guidlines. Use '{snakeCase}' instead.");
            }
        }
    }

    [MenuItem("Tools/Inventory manager/Analytics/Analyze item set definitions")]
    public static void AnalyzeAssetsDefinitions()
    {
        var allItems = getAssets();

        var missingMaterial = allItems.Where(x => x.ItemData.Material == null);
        var missingMesh = allItems.Where(x => x.ItemData.Mesh == null);

        Debug.ClearDeveloperConsole();
        foreach (var item in missingMaterial)
        {
            Debug.LogErrorFormat(
                "Item '{0}' (path: {1}) is missing material.",
                item.ItemData.InternalName,
                item.Path
                );
        }
        foreach (var item in missingMesh)
        {
            Debug.LogErrorFormat(
                "Item '{0}' (path: {1}) is missing mesh.",
                item.ItemData.InternalName,
                item.Path
                );
        }
    }

    [MenuItem("Tools/Inventory manager/Analytics/Analyze item obtainability")]
    public static void AnalyzeAssetsObtainability()
    {
        var allItems = getAssets();

        //TODO:
    }

    private static List<AnalyzeItemDataDto> getAssets()
    {
        List<AnalyzeItemDataDto> allItems = new List<AnalyzeItemDataDto>();
        foreach (var id in AssetDatabase.FindAssets("t:ItemData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            allItems.Add(new AnalyzeItemDataDto(sourceAsset, path));
        }
        return allItems;
    }

#endif
}
