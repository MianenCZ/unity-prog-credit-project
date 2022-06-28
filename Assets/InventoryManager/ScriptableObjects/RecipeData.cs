using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;



[CreateAssetMenu(fileName = "new_recipe", menuName = "Inventory Manager/Recipe")]
public class RecipeData : ScriptableObject
{
    [HideInInspector]
    [Tooltip("List of input items with amounts. Note: Use at least one item and nonzero Amounts")]
    public List<ItemAmountListing> Source;

    [HideInInspector]
    [Tooltip("List of output items with amounts. Note: Use at least one item and nonzero Amounts")]
    public List<ItemAmountListing> Results;

    private static string ResourcePath => @"Assets\Resources\Recipes";


#if UNITY_EDITOR

    [MenuItem("Tools/Inventory manager/Create new recipe")]
    public static void CreateNewItem()
    {
        var path = Path.Combine(ResourcePath, "recipe.asset");
        var data = CreateInstance<RecipeData>();
        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(data, path);
    }

    [MenuItem("Assets/Inventory manager/Combine items to recipe")]
    public static void CombineItem()
    {
        var selectedItems = new List<ItemData>();
        foreach (var assetGuid in Selection.assetGUIDs)
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var sourceAsset = (ItemData)AssetDatabase.LoadAssetAtPath(clickedPath, typeof(ItemData));

            if (sourceAsset == null)
            {
                Debug.LogError($"Only '{typeof(ItemData)}' can be combined to '{typeof(RecipeData)}'.");
                return;
            }
            selectedItems.Add(sourceAsset);
        }

        var recipe = CreateInstance<RecipeData>();
        recipe.Source = selectedItems.Select(x => new ItemAmountListing 
        {
            ItemData = x,
            Amount = 1,    
        }).ToList();
        recipe.Results = new List<ItemAmountListing>();

        var search = AssetDatabase.FindAssets("recipe_new_rename_me_ t:RecipeData");

        var path = Path.Combine(ResourcePath, $"recipe_new_rename_me_{search.Length}.asset");
        var testPath = AssetDatabase.AssetPathToGUID(path);
        Debug.Log(testPath);
        if (!string.IsNullOrEmpty(testPath))
        {
            Debug.LogError($"Asset on path '{path}' already exists.");
            return;
        }

        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(recipe, path);
    }


    /// <summary>
    /// Returns all scriptable objects of type <see cref="RecipeData"/>.
    /// </summary>
    /// <returns>Loaded scriptable objects</returns>
    public static List<RecipeData> GetAllRecipes()
    {
        List<RecipeData> allItems = new List<RecipeData>();
        foreach (var id in AssetDatabase.FindAssets("t:RecipeData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
            allItems.Add(sourceAsset);
        }
        return allItems;
    }

    private static List<RecipeData> _getAllCache = null;

    /// <summary>
    /// Returns all scriptable objects of type <see cref="RecipeData"/>. Load is executed only once, on first call.
    /// </summary>
    /// <returns>Loaded scriptable objects</returns>
    public static List<RecipeData> GetAllItemsCached()
    {
        if (_getAllCache == null)
            _getAllCache = GetAllRecipes();
        return _getAllCache;
    }


#endif
}
