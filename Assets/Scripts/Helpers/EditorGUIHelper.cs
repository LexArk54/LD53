#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorGUIHelper {

    public static SerializedProperty FindPropertyRelative(this SerializedObject serializedObject, string findByName) {
        var prop = serializedObject.GetIterator();
        if (prop.NextVisible(true)) {
            do {
                if (prop.name == findByName) break;
            } while (prop.NextVisible(false));
        }
        return prop;
    }

    public static SerializedProperty DrawInline(this SerializedProperty property, ref Rect rect, int width) {
        rect.width = width;
        EditorGUI.PropertyField(rect, property, GUIContent.none);
        rect.x += width;
        return property;
    }

    public static void DrawInline(ref Rect rect, SerializedProperty prop, int width) {
        rect.width = width;
        EditorGUI.PropertyField(rect, prop, GUIContent.none);
        rect.x += width;
    }

    public static void DrawInlineLabel(ref Rect rect, string title, int width) {
        rect.width = width;
        EditorGUI.LabelField(rect, title);
        rect.x += width;
    }

    public static void DrawInlineStringPopup(this SerializedProperty property, ref Rect rect, string[] items, int width) {
        var valueIndex = Array.IndexOf(items, property.stringValue);
        rect.width = width;
        var newIndexValue = EditorGUI.Popup(rect, valueIndex, items);
        rect.x += width;
        if (newIndexValue != valueIndex && items.Length > newIndexValue) {
            property.stringValue = items[newIndexValue];
        }
    }

}
#endif