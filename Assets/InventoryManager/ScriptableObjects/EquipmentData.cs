using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum EquipmentSlot
{
    ArmLowerLeft,
    ArmLowerRight,
    ArmUpperLeft,
    ArmUpperRight,
    BackAttachment,
    Ear,
    ElbowAttachLeft,
    ElbowAttachRight,
    Eyebrow,
    FacialHair,
    Hair,
    HandLeft,
    HandRight,
    Head,
    HeadCoverings,
    HelmetAttachment,
    Hips,
    HipsAttachment,
    KneeAttachLeft,
    KneeAttachRight,
    LegLeft,
    LegRight,
    ShoulderAttachLeft,
    ShoulderAttachRight,
    Torso,
}

public enum Gender
{
    Male,
    Female,
    Universal
}

public class EquipmentData : ItemData
{
    public EquipmentSlot EquipmentSlot;
    public Gender Gender;

    private static string ResourcePath => @"Assets\Resources\Equipment";


#if UNITY_EDITOR


    [MenuItem("Tools/Inventory manager/Create new equipment")]
    public static void CreateNewEquipment()
    {
        var path = Path.Combine(ResourcePath, "equipment.asset");
        var data = CreateInstance<ItemData>();
        ProjectWindowUtil.CreateAsset(data, path);
    }


    [MenuItem("Assets/Inventory manager/Convert asset to equipment")]
    public static void ConvertToEquipment()
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

            var data = CreateInstance<EquipmentData>();
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

    [MenuItem("Assets/Inventory manager/SYNTY STUDIOS/Get modular armor types")]
    public static void SyntyStudiosGetModularArmorTypes()
    {
        var selection = Selection.assetGUIDs;
        var names = new List<string>();
        foreach (var assetGuid in selection)
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var sourceAsset = AssetDatabase.LoadAssetAtPath<GameObject>(clickedPath);

            names.Add(sourceAsset.name);
        }

        //Debug.Log(string.Join(", ", names.Take(10)));
        Debug.Log(string.Join(", ", names.Where(x => x.Contains('_')).Select(x => x.Split('_')[1]).Distinct()));

    }

    [MenuItem("Assets/Inventory manager/SYNTY STUDIOS/Convert asset to equipment")]
    public static void SyntyStudiosConvertAssetToEquipment()
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

            var data = CreateInstance<EquipmentData>();
            var trimName = TrimName(sourceAsset.name);

            data.Name = trimName.newName;
            data.InternalName = trimName.internalName;
            data.Gender = trimName.gender;
            data.EquipmentSlot = trimName.equipmentSlot;
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

    private static (string newName, string internalName, Gender gender, EquipmentSlot equipmentSlot) TrimName(string name)
    {
        var equipmentSlot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), name.Split('_')[1]);
        var gender = name.Contains(Gender.Male.ToString()) ? Gender.Male : (name.Contains(Gender.Female.ToString()) ? Gender.Female : Gender.Universal);

        var newName =
            name.Replace('_', ' ')
                .Replace("Chr", "")
                .Replace("Static", "")
                .Trim(' ');
        var internalName = newName.Replace(' ', '_').ToLower();

        return (newName, internalName, gender, equipmentSlot);
    }

#endif

}
