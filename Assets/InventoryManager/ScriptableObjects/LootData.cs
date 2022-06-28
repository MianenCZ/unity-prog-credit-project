using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;

/// <summary>
/// Draws the field/property ONLY if the copared property compared by the comparison type with the value of comparedValue returns true.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DrawIfLootEntryModeAttribute : PropertyAttribute
{
    public int comparedValue { get; private set; }

    /// <summary>
    /// Only draws the field only if a condition is met.
    /// </summary>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    public DrawIfLootEntryModeAttribute(LootEntry.Distribution comparedValue)
    {
        this.comparedValue = (int)comparedValue;
    }
}

public class ShowWhenAttribute : PropertyAttribute
{
    public string PropertyName { get; }
    public object Value { get; }

    public ShowWhenAttribute(string propertyName, object value)
    {
        PropertyName = propertyName;
        Value = value;
    }
}

[System.Serializable]
public class LootEntry
{
    public enum Distribution
    {
        Constant,
        Uniform,
        Binomial,
    }

    [Tooltip("")]
    public ItemData ItemData;
    [Tooltip("")]
    public float Weight;
    [Tooltip("After first success removes this LooEntry from loot pool.")]
    public bool OncePerPool;

    [Space]
    [Tooltip("")]
    public ItemSource TargetedSource;


    [Header("Inner distribution of loot entry")]
    [Tooltip("Inner distribution mode")]
    public Distribution Mode;

    //Constant mode
    [DrawIfLootEntryMode(Distribution.Constant)]
    [Tooltip("Exact amount of drops")]
    public int Value;

    //Uniform mode
    [DrawIfLootEntryMode(Distribution.Uniform)]
    [Tooltip("Minimal number of drops (value included)")]
    public int Min;
    [DrawIfLootEntryMode(Distribution.Uniform)]
    [Tooltip("Maximal number of drops (value included)")]
    public int Max;

    //Binomial mode
    [DrawIfLootEntryMode(Distribution.Binomial)]
    [Tooltip("Number of tries")]
    public int Tries;
    [DrawIfLootEntryMode(Distribution.Binomial)]
    [Tooltip("Success probability for each trial")]
    public float SuccessProbability;

    public string InternalDistributionToString()
    {
        switch (Mode)
        {
            case Distribution.Constant: return $"{Value}";
            case Distribution.Uniform: return $"{Min}-{Max}";
            case Distribution.Binomial: return $"{Tries}x {SuccessProbability * 100}%";
            default: return "";
        }
    }

}


[CreateAssetMenu(fileName = "new_loot", menuName = "Inventory Manager/Loot table")]
public class LootData : ScriptableObject
{
    [Tooltip("Number of cycles of calculating final loot. Note: Should be a positive number")]
    public int Rolls;
    [Tooltip("Collection of entries to calculate final loot from. Note: Should not be empty")]
    public List<LootEntry> LootEntries;

    private static string ResourcePath => @"Assets\Resources\LootTables";


#if UNITY_EDITOR
    [MenuItem("Tools/Inventory manager/Create new loot table")]
    public static void CreateNewLootTable()
    {
        var path = Path.Combine(ResourcePath, "loot_table.asset");
        var data = CreateInstance<LootData>();
        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(data, path);
    }

    [MenuItem("Assets/Inventory manager/Combine items to loot table")]
    public static void CombineItemsToLootTable()
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

        var recipe = CreateInstance<LootData>();
        recipe.LootEntries = selectedItems.Select(x => new LootEntry
        {
            ItemData = x,
            Mode = LootEntry.Distribution.Constant,
            Value = 1,
            Weight = 1,
            OncePerPool = false,
        }).ToList();
        recipe.Rolls = 1;


        var path = Path.Combine(ResourcePath, $"recipe_new_rename_me.asset");
        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(recipe, path);
    }

    [MenuItem("Assets/Inventory manager/Combine items to full drop")]
    public static void CombineItemsToDrop()
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

        var recipe = CreateInstance<LootData>();
        recipe.LootEntries = selectedItems.Select(x => new LootEntry
        {
            ItemData = x,
            Mode = LootEntry.Distribution.Constant,
            Value = 1,
            Weight = 1,
            OncePerPool = true,
        }).ToList();
        recipe.Rolls = selectedItems.Count;


        var path = Path.Combine(ResourcePath, $"recipe_new_rename_me.asset");
        Directory.CreateDirectory(ResourcePath);
        ProjectWindowUtil.CreateAsset(recipe, path);
    }

    /// <summary>
    /// Returns all scriptable objects of type <see cref="LootData"/>
    /// </summary>
    /// <returns>Loaded scriptable objects</returns>
    public static List<LootData> GetAllLootTables()
    {
        List<LootData> allItems = new List<LootData>();
        foreach (var id in AssetDatabase.FindAssets("t:LootData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<LootData>(path);
            allItems.Add(sourceAsset);
        }
        return allItems;
    }

    private static List<LootData> _getAllCache = null;

    /// <summary>
    /// Returns all scriptable objects of type <see cref="LootData"/>. Load is executed only once, on first call.
    /// </summary>
    /// <returns>Loaded scriptable objects</returns>
    public static List<LootData> GetAllItemsCached()
    {
        if (_getAllCache == null)
            _getAllCache = GetAllLootTables();
        return _getAllCache;
    }

#endif

    public static List<ItemAmountListing> CalculateLoot(LootData lootData)
    {
        Dictionary<string, ItemAmountListing> result = new Dictionary<string, ItemAmountListing>();

        for (int roll = 0; roll < lootData.Rolls; roll++)
        {
            // agregate base set of lootable data
            var lootable = lootData.LootEntries.Where(x => !x.OncePerPool || !result.ContainsKey(x.ItemData.InternalName)).ToList();
            var totalWeight = lootable.Sum(x => x.Weight);

            // select item
            float rndValue = UnityEngine.Random.Range(0f, totalWeight);

            // find loot
            float sum = 0f;
            LootEntry newLoot = null;
            foreach (var item in lootable)
            {
                sum += item.Weight;
                if(sum > rndValue)
                {
                    newLoot = item;
                    break;
                }
            }

            int lootItemAmount = 0;
            // calculate inner distribution
            switch (newLoot.Mode)
            {
                case LootEntry.Distribution.Constant:
                    lootItemAmount = newLoot.Value;
                    break;
                case LootEntry.Distribution.Uniform:
                    lootItemAmount = UnityEngine.Random.Range(newLoot.Min, newLoot.Max);
                    break;
                case LootEntry.Distribution.Binomial:
                    for (int tries = 0; tries < newLoot.Tries; tries++)
                        lootItemAmount += (UnityEngine.Random.Range(0f, 1f) > newLoot.SuccessProbability) ? 1 : 0;
                    break;
            }

            if (!result.ContainsKey(newLoot.ItemData.InternalName))
                result[newLoot.ItemData.InternalName] = new ItemAmountListing(newLoot.ItemData, 0);

            result[newLoot.ItemData.InternalName].Amount += lootItemAmount;
        }

        return result.Values.ToList();
    }
}