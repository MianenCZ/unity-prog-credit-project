using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(EquipmentData))]
public class EquipmentDataEditor : Editor
{
    SerializedProperty fullNameProperty;
    SerializedProperty internalNameProperty;
    SerializedProperty stackableProperty;
    SerializedProperty stackSizeProperty;
    SerializedProperty dropableProperty;
    SerializedProperty categoryProperty;
    SerializedProperty sourceProperty;
    SerializedProperty meshProperty;
    SerializedProperty materialProperty;
    SerializedProperty equipmentIconScaleProperty;
    SerializedProperty equipmentIconOffsetProperty;
    SerializedProperty equipmentIconRotationProperty;
    SerializedProperty equipmentIngameScaleProperty;
    SerializedProperty equipmentIngameOffsetProperty;
    SerializedProperty equipmentEditorIconScale;
    SerializedProperty equipmentEditorIconOffset;
    SerializedProperty equipmentEditorIconRotation;

    bool showAddtional3DData = false;

    private PreviewRenderUtility previewRenderer;
    private InventoryManagerSettings settings;

    void OnEnable()
    {
        settings = InventoryManagerSettings.GetActiveSettings();

        previewRenderer = new PreviewRenderUtility();
        previewRenderer.camera.backgroundColor = settings.BackgroundColor;
        previewRenderer.camera.clearFlags = settings.CameraClearFlags;
        previewRenderer.camera.transform.position = new Vector3(0,0,-10f);

        fullNameProperty = serializedObject.FindProperty(nameof(EquipmentData.Name));
        internalNameProperty = serializedObject.FindProperty(nameof(EquipmentData.InternalName));
        stackableProperty = serializedObject.FindProperty(nameof(EquipmentData.Stackable));
        stackSizeProperty = serializedObject.FindProperty(nameof(EquipmentData.StackSize));
        dropableProperty = serializedObject.FindProperty(nameof(EquipmentData.Dropable));
        categoryProperty = serializedObject.FindProperty(nameof(EquipmentData.Category));
        sourceProperty = serializedObject.FindProperty(nameof(EquipmentData.Source));
        meshProperty = serializedObject.FindProperty(nameof(EquipmentData.Mesh));
        materialProperty = serializedObject.FindProperty(nameof(EquipmentData.Material));
        equipmentIconScaleProperty = serializedObject.FindProperty(nameof(EquipmentData.ItemIconScale));
        equipmentIconOffsetProperty = serializedObject.FindProperty(nameof(EquipmentData.ItemIconOffset));
        equipmentIconRotationProperty = serializedObject.FindProperty(nameof(EquipmentData.ItemIconRotation));
        equipmentIngameScaleProperty = serializedObject.FindProperty(nameof(EquipmentData.ItemIngameScale));
        equipmentIngameOffsetProperty = serializedObject.FindProperty(nameof(EquipmentData.ItemIngameOffset));
        equipmentEditorIconScale = serializedObject.FindProperty(nameof(EquipmentData.EditorIconScale));
        equipmentEditorIconOffset = serializedObject.FindProperty(nameof(EquipmentData.EditorIconOffset));
        equipmentEditorIconRotation = serializedObject.FindProperty(nameof(EquipmentData.EditorIconRotation));
    }

    private void OnDisable()
    {
        if (previewRenderer != null)
        {
            previewRenderer.Cleanup();
            previewRenderer = null;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var mesh = meshProperty.Value<Mesh>();
        var material = materialProperty.Value<Material>();
        var scale = equipmentEditorIconScale.Value<Vector3>();
        var offset = equipmentEditorIconOffset.Value<Vector3>();
        if (targets.Length == 1 && mesh != null && material != null)
        {
            var width = Mathf.Min(EditorGUIUtility.currentViewWidth, settings.DetailPreviewMaxImageSize);
            var centering = (EditorGUIUtility.currentViewWidth - width) / 2;
            var bound = new Rect(centering, 0, width, width);
            previewRenderer.BeginPreview(bound, GUIStyle.none);
            previewRenderer.DrawMesh(mesh, 
                Matrix4x4.identity * Matrix4x4.Scale(scale) * Matrix4x4.Translate(offset) * Matrix4x4.Rotate(Quaternion.Euler(Vector3.up * 180f)), material, 0);
            previewRenderer.camera.Render();
            var render = previewRenderer.EndPreview();
            GUI.DrawTexture(bound, render);
            EditorGUILayout.Space(width);
        }

        EditorGUILayout.PropertyField(fullNameProperty);
        EditorGUILayout.PropertyField(internalNameProperty);
        EditorGUILayout.PropertyField(categoryProperty);
        EditorGUILayout.PropertyField(sourceProperty);
        EditorGUILayout.PropertyField(stackableProperty);
        if (stackableProperty.boolValue)
        {
            EditorGUILayout.PropertyField(stackSizeProperty);
        }
        EditorGUILayout.PropertyField(dropableProperty);
        EditorGUILayout.PropertyField(meshProperty);
        EditorGUILayout.PropertyField(materialProperty);

        showAddtional3DData = EditorGUILayout.Foldout(showAddtional3DData, "Aditional data", true);
        if (showAddtional3DData)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(equipmentIconScaleProperty);
            EditorGUILayout.PropertyField(equipmentIconOffsetProperty);
            EditorGUILayout.PropertyField(equipmentIconRotationProperty);
            EditorGUILayout.PropertyField(equipmentIngameScaleProperty);
            EditorGUILayout.PropertyField(equipmentIngameOffsetProperty);
            EditorGUILayout.PropertyField(equipmentEditorIconScale);
            EditorGUILayout.PropertyField(equipmentEditorIconOffset);
            EditorGUILayout.PropertyField(equipmentEditorIconRotation);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}
