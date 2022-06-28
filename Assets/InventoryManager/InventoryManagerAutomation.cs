using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class InventoryManagerAutomation
{

#if UNITY_EDITOR
    [MenuItem("Tools/Inventory manager/Automation/Recipes to ItemData Souce flags")]
    public static void RecipesToItemDataSource()
    {
        var recipes = RecipeData.GetAllItemsCached();

        HashSet<string> recipeItems = new HashSet<string>();
        foreach (var recipe in recipes)
        {
            foreach (var item in recipe.Results.Where(x => x.ItemData != null))
            {
                recipeItems.Add(item.ItemData.InternalName);
                item.ItemData.Source |= ItemSource.Crafting;
            }
        }

        var items = ItemData.GetAllItems();
        foreach (var item in items)
        {
            if (!recipeItems.Contains(item.InternalName))
            {
                item.Source &= ~ItemSource.Crafting;
            }
        }

        AssetDatabase.SaveAssets();
        ItemData.RefreshItemCache();
    }

    [MenuItem("Tools/Inventory manager/Automation/Loot tables to ItemData Souce flags")]
    public static void LootTableToItemDataSource()
    {
        var loots = LootData.GetAllItemsCached();

        foreach (var loot in loots)
        {
            foreach (var lootEntry in loot.LootEntries)
            {
                lootEntry.ItemData.Source |= lootEntry.TargetedSource;
            }
        }

        AssetDatabase.SaveAssets();
        ItemData.RefreshItemCache();
    }

    [MenuItem("Tools/Inventory manager/Automation/Clear ItemData Source")]
    public static void ClearItemSourceFlags()
    {
        var items = ItemData.GetAllItems();
        foreach (var item in items)
        {
            item.Source = ItemSource.None;
        }

        AssetDatabase.SaveAssets();
        ItemData.RefreshItemCache();
    }
#endif
}