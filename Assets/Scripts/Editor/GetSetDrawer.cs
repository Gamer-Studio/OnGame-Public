#if UNITY_EDITOR
using System.Reflection;
using OnGame.Utils;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GetSetAttribute))]
public sealed class GetSetDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    var attribute = (GetSetAttribute)this.attribute;

    EditorGUI.BeginChangeCheck();
    EditorGUI.PropertyField(position, property, label);

    if (EditorGUI.EndChangeCheck())
    {
      attribute.dirty = true;
    }
    else if (attribute.dirty)
    {
      var parent = GetParentObject(property.propertyPath, property.serializedObject.targetObject);

      var type = parent.GetType();
      var info = type.GetProperty(attribute.name);

      if (info == null)
        Debug.LogError("Invalid property name \"" + attribute.name + "\"");
      else
        info.SetValue(parent, fieldInfo.GetValue(parent), null);

      attribute.dirty = false;
    }
  }

  public static object GetParentObject(string path, object obj)
  {
    var fields = path.Split('.');

    if (fields.Length == 1)
      return obj;

    var info = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    obj = info.GetValue(obj);

    return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
  }
}
#endif