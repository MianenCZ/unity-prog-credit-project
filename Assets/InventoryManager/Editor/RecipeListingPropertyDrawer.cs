using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemAmountListing))]
public class RecipeListingPropertyDrawer : PropertyDrawer
{
    private const float AmountSize = 50f;
    private const float Space = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sourceProperty = property.FindPropertyRelative(nameof(ItemAmountListing.ItemData)); ;
        var amountProperty = property.FindPropertyRelative(nameof(ItemAmountListing.Amount)); ;


        EditorGUI.BeginProperty(position, label, property);
        label.text = "";

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        (Rect sourcePos, Rect amountPos) = ComputePositions(position);
        EditorGUI.PropertyField(sourcePos, sourceProperty, GUIContent.none);
        EditorGUI.PropertyField(amountPos, amountProperty, GUIContent.none);
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    private (Rect sourcePos, Rect amountPos) ComputePositions(Rect position)
    {
        Rect sourcePos = new Rect(position.x, position.y, position.width - (AmountSize + Space) , position.height);
        Rect amountPos = new Rect(position.x + sourcePos.width + Space, position.y, AmountSize, position.height);
        return (sourcePos, amountPos);
    }
}
