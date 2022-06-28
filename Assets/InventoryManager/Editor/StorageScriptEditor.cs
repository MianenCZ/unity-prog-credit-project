using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(StorageScript))]
public class StorageScriptEditor : Editor
{
    SerializedProperty slotCountProperty;
    SerializedProperty itemsProperty;
    PreviewRenderUtility previewRenderer;
    InventoryManagerSettings settings;

    private void OnEnable()
    {
        slotCountProperty = serializedObject.FindProperty(nameof(StorageScript.SlotCount));
        itemsProperty = serializedObject.FindProperty(nameof(StorageScript.Items));


        settings = InventoryManagerSettings.GetActiveSettings();

        previewRenderer = new PreviewRenderUtility();
        previewRenderer.camera.backgroundColor = settings.BackgroundColor;
        previewRenderer.camera.clearFlags = settings.CameraClearFlags;
        previewRenderer.camera.transform.position = settings.CameraPosition;
    }

    public override void OnInspectorGUI()
    {
        if (targets.Length != 1)
        {
            base.OnInspectorGUI();
            return;
        }


        var items = itemsProperty.Value<List<ItemAmountListing>>();
        DrawListing(items, 0f, 0f);



        base.OnInspectorGUI();
    }

    private void DrawListing(List<ItemAmountListing> items, float xOffset, float yOffset)
    {
        var space = settings.ListPreviewSpaceSize;
        var width = settings.ListPreviewImageSize;

        int columnCount = (int)(EditorGUIUtility.currentViewWidth - space) / (int)(width + space);
        var rowCentering = (EditorGUIUtility.currentViewWidth - (columnCount * width) - ((columnCount - 1) * space)) / 2f;
        var rowCount = Mathf.CeilToInt((float)items.Count / columnCount);

        for (int i = 0; i < items.Count; i++)
        {
            var bound = new Rect(
                (i % columnCount) * (width) + (i % columnCount) * space + rowCentering + xOffset,
                (i / columnCount) * (width + space) + space + yOffset,
                width,
                width);

            previewRenderer.BeginPreview(bound, GUIStyle.none);
            if (items[i] != null && items[i].ItemData != null && items[i].ItemData.Mesh != null && items[i].ItemData.Material != null)
                previewRenderer.DrawMesh(
                    items[i].ItemData.Mesh,
                    Matrix4x4.identity * Matrix4x4.Scale(items[i].ItemData.EditorIconScale) * Matrix4x4.Translate(items[i].ItemData.EditorIconOffset) * Matrix4x4.Rotate(Quaternion.Euler(items[i].ItemData.EditorIconRotation)), 
                    items[i].ItemData.Material, 
                    0);
            previewRenderer.camera.Render();
            var render = previewRenderer.EndPreview();
            GUI.DrawTexture(bound, render);

            var labelBound = new Rect(bound);
            labelBound.x += settings.LabelOffset.x;
            labelBound.y += settings.LabelOffset.y;


            var textStyle = new GUIStyle();
            textStyle.fontSize = settings.LabelFontSize;
            textStyle.fontStyle = settings.LabelFontStyle;
            textStyle.normal.textColor = settings.LableColor;
            EditorGUI.LabelField(labelBound, items[i].Amount.ToString(), textStyle);
        }
        EditorGUILayout.Space(rowCount * (width + space));
    }

    private void OnDisable()
    {
        previewRenderer.Cleanup();
    }
}
