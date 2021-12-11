﻿using RoR2;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace RoR2EditorKit.RoR2.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public class EnumMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Enum targetEnum = GetBaseProperty<Enum>(property);

            string propName = property.name;
            if (string.IsNullOrEmpty(propName))
                propName = Regex.Replace(property.name, "([^^])([A-Z])", "$1 $2");
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);

            Enum enumNew = EditorGUI.EnumFlagsField(position, ObjectNames.NicifyVariableName(propName), targetEnum);

            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
            {
                var convertedType = Convert.ChangeType(enumNew, targetEnum.GetType());
                property.intValue = Convert.ToInt32(convertedType);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
            }
        }

        static T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
            return (T)reflectionTarget;
        }
    }
}