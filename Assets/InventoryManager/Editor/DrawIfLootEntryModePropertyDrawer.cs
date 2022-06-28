using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfLootEntryModeAttribute))]
public class DrawIfLootEntryModePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ConditionMet(property))
            return base.GetPropertyHeight(property, label);
        else
            return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ConditionMet(property))
            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
    }

    private bool ConditionMet(SerializedProperty property)
    {
        var drawIf = attribute as DrawIfLootEntryModeAttribute;
        var pathSplit = property.propertyPath.Split('.');
        var newPath = string.Join(".", pathSplit.Take(pathSplit.Length - 1));
        var comparedField = property.serializedObject.FindProperty($"{newPath}.{nameof(LootEntry.Mode)}");
        int comparedFieldValue = comparedField.enumValueIndex;
        return comparedFieldValue == drawIf.comparedValue;
    }
}