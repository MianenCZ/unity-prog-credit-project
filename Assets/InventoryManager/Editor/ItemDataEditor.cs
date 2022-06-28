using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
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
    SerializedProperty itemIconScaleProperty;
    SerializedProperty itemIconOffsetProperty;
    SerializedProperty itemIconRotationProperty;
    SerializedProperty itemIngameScaleProperty;
    SerializedProperty itemIngameOffsetProperty;
    SerializedProperty itemEditorIconScale;
    SerializedProperty itemEditorIconOffset;
    SerializedProperty itemEditorIconRotation;

    bool showAddtional3DData = false;

    private PreviewRenderUtility previewRenderer;
    private InventoryManagerSettings settings;

    void OnEnable()
    {
        settings = InventoryManagerSettings.GetActiveSettings();

        previewRenderer = new PreviewRenderUtility();
        previewRenderer.camera.backgroundColor = settings.BackgroundColor;
        previewRenderer.camera.clearFlags = settings.CameraClearFlags;
        previewRenderer.camera.transform.position = settings.CameraPosition;

        fullNameProperty = serializedObject.FindProperty(nameof(ItemData.Name));
        internalNameProperty = serializedObject.FindProperty(nameof(ItemData.InternalName));
        stackableProperty = serializedObject.FindProperty(nameof(ItemData.Stackable));
        stackSizeProperty = serializedObject.FindProperty(nameof(ItemData.StackSize));
        dropableProperty = serializedObject.FindProperty(nameof(ItemData.Dropable));
        categoryProperty = serializedObject.FindProperty(nameof(ItemData.Category));
        sourceProperty = serializedObject.FindProperty(nameof(ItemData.Source));
        meshProperty = serializedObject.FindProperty(nameof(ItemData.Mesh));
        materialProperty = serializedObject.FindProperty(nameof(ItemData.Material));
        itemIconScaleProperty = serializedObject.FindProperty(nameof(ItemData.ItemIconScale));
        itemIconOffsetProperty = serializedObject.FindProperty(nameof(ItemData.ItemIconOffset));
        itemIconRotationProperty = serializedObject.FindProperty(nameof(ItemData.ItemIconRotation));
        itemIngameScaleProperty = serializedObject.FindProperty(nameof(ItemData.ItemIngameScale));
        itemIngameOffsetProperty = serializedObject.FindProperty(nameof(ItemData.ItemIngameOffset));
        itemEditorIconScale = serializedObject.FindProperty(nameof(ItemData.EditorIconScale));
        itemEditorIconOffset = serializedObject.FindProperty(nameof(ItemData.EditorIconOffset));
        itemEditorIconRotation = serializedObject.FindProperty(nameof(ItemData.EditorIconRotation));
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
        var scale = itemEditorIconScale.Value<Vector3>();
        var offstet =  itemEditorIconOffset.Value<Vector3>();
        var rotation =  itemEditorIconRotation.Value<Vector3>();
        if (targets.Length == 1 && mesh != null && material != null)
        {
            var width = Mathf.Min(EditorGUIUtility.currentViewWidth, settings.DetailPreviewMaxImageSize);
            var centering = (EditorGUIUtility.currentViewWidth - width) / 2;
            var bound = new Rect(centering, 0, width, width);
            previewRenderer.BeginPreview(bound, GUIStyle.none);
            previewRenderer.DrawMesh(
                mesh, 
                Matrix4x4.identity * Matrix4x4.Scale(scale) * Matrix4x4.Translate(offstet) * Matrix4x4.Rotate(Quaternion.Euler(rotation)), 
                material, 
                0);
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

        showAddtional3DData = EditorGUILayout.Foldout(showAddtional3DData, "Aditional data");
        if (showAddtional3DData)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(itemIconScaleProperty);
            EditorGUILayout.PropertyField(itemIconOffsetProperty);
            EditorGUILayout.PropertyField(itemIconRotationProperty);
            EditorGUILayout.PropertyField(itemIngameScaleProperty);
            EditorGUILayout.PropertyField(itemIngameOffsetProperty);
            EditorGUILayout.PropertyField(itemEditorIconScale);
            EditorGUILayout.PropertyField(itemEditorIconOffset);
            EditorGUILayout.PropertyField(itemEditorIconRotation);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}

public static class SerializedPropertyExtension
{
    public static T Value<T>(this SerializedProperty serializedProperty)
    {
        if (serializedProperty == null)
            throw new System.ArgumentNullException(nameof(serializedProperty));

        var targetObject = serializedProperty.serializedObject.targetObject;
        var targetObjectClassType = targetObject.GetType();
        var field = targetObjectClassType.GetField(serializedProperty.propertyPath);

        if (field == null)
            return default(T);
        var value = field.GetValue(targetObject);

        if(value is T)
            return (T)value;
        return default(T);
    }
}
