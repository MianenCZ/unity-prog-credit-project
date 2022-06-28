using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LootData))]
public class LootDataEditor : Editor
{
    SerializedProperty lootEntriesProperty;
    PreviewRenderUtility previewRenderer;
    InventoryManagerSettings settings;

    private void OnEnable()
    {
        lootEntriesProperty = serializedObject.FindProperty(nameof(LootData.LootEntries));

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

        var lootEntries = lootEntriesProperty.Value<List<LootEntry>>();

        var off = DrawListing(lootEntries, 0f, 0f);


        base.OnInspectorGUI();
    }

    private (float xOffset, float yOffset) DrawListing(List<LootEntry> items, float xOffset, float yOffset)
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

            EditorGUI.LabelField(labelBound, items[i].InternalDistributionToString(), textStyle);
        }
        float height = rowCount * (width + space);
        EditorGUILayout.Space(height);
        return (0f, height);
    }
}
