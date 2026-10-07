using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InterfaceRefs.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceRef<>))]
    public class InterfaceRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative("target");
            Type interfaceType = GetInterfaceType(property);

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                Rect fieldRect = EditorGUI.PrefixLabel(position, label);

                EditorGUI.BeginChangeCheck();

                Object picked = EditorGUI.ObjectField(
                    fieldRect,
                    targetProperty.objectReferenceValue,
                    typeof(Object),
                    allowSceneObjects: true
                );

                if (EditorGUI.EndChangeCheck())
                {
                    targetProperty.objectReferenceValue = Resolve(picked, interfaceType);
                }
            }
        }

        Type GetInterfaceType(SerializedProperty property)
        {
            object boxedValue = property.boxedValue;
            Type structType = boxedValue.GetType();
            return structType.GetGenericArguments()[0];
        }

        Object Resolve(Object picked, Type interfaceType)
        {
            if (picked == null)
            {
                return null;
            }

            if (interfaceType.IsInstanceOfType(picked))
            {
                return picked;
            }

            if (picked is GameObject gameObject)
            {
                return gameObject.GetComponent(interfaceType);
            }

            return null;
        }
    }
}
