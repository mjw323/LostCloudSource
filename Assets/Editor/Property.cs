using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class Property {
	private System.Object instance;
	private PropertyInfo info;
	private SerializedPropertyType type;
	private MethodInfo getter;
	private MethodInfo setter;

	public SerializedPropertyType Type {
		get { return type; }
	}

	public String Name {
		get { return ObjectNames.NicifyVariableName(info.Name); }
	}

	public Property(System.Object instance, PropertyInfo info,
	                SerializedPropertyType type) {
		this.instance = instance;
		this.info = info;
		this.type = type;
		getter = info.GetGetMethod();
		setter = info.GetSetMethod();
	}

	public System.Object GetValue() {
		return getter.Invoke(instance, null);
	}

	public void SetValue(System.Object value) {
		setter.Invoke(instance, new System.Object[]{value});
	}

	public static void ExposeProperties(Property[] properties) {
		GUILayoutOption[] emptyOptions = new GUILayoutOption[0];

		EditorGUILayout.BeginVertical();
		for (int i = 0; i < properties.Length; i++) {
			EditorGUILayout.BeginHorizontal();
			switch (properties[i].Type) {
			case SerializedPropertyType.Integer:
				properties[i].SetValue(EditorGUILayout.IntField(
					properties[i].Name, (int)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Boolean:
				properties[i].SetValue(EditorGUILayout.Toggle(
					properties[i].Name, (bool)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Float:
				properties[i].SetValue(EditorGUILayout.FloatField(
					properties[i].Name, (float)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.String:
				properties[i].SetValue(EditorGUILayout.TextField(
					properties[i].Name, (string)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Color:
				properties[i].SetValue(EditorGUILayout.ColorField(
					properties[i].Name, (Color)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.LayerMask:
				properties[i].SetValue(EditorGUILayout.LayerField(
					properties[i].Name, (LayerMask)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Enum:
				properties[i].SetValue(EditorGUILayout.EnumPopup(
					properties[i].Name, (Enum)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Vector2:
				properties[i].SetValue(EditorGUILayout.Vector2Field(
					properties[i].Name, (Vector2)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Vector3:
				properties[i].SetValue(EditorGUILayout.Vector3Field(
					properties[i].Name, (Vector3)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Rect:
				properties[i].SetValue(EditorGUILayout.RectField(
					properties[i].Name, (Rect)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.AnimationCurve:
				properties[i].SetValue(EditorGUILayout.CurveField(
					properties[i].Name,(AnimationCurve)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.Bounds:
				properties[i].SetValue(EditorGUILayout.BoundsField(
					properties[i].Name, (Bounds)properties[i].GetValue(),
					emptyOptions));
				break;
			case SerializedPropertyType.ObjectReference:
				properties[i].SetValue(EditorGUILayout.ObjectField(
					properties[i].Name,
					(UnityEngine.Object)properties[i].GetValue(),
					properties[i].info.PropertyType,
					true, emptyOptions));
				break;
			default:
				break;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}

	public static bool GetType(PropertyInfo info,
	                           out SerializedPropertyType propertyType) {
		propertyType = SerializedPropertyType.Generic;
		Type type = info.PropertyType;
		if (type == typeof(int)) {
			propertyType = SerializedPropertyType.Integer;
			return true;
		}
		if (type == typeof(bool)) {
			propertyType = SerializedPropertyType.Boolean;
			return true;
		}
		if (type == typeof(float)) {
			propertyType = SerializedPropertyType.Float;
			return true;
		}
		if (type == typeof(string)) {
			propertyType = SerializedPropertyType.String;
			return true;
		}
		if (type == typeof(Color)) {
			propertyType = SerializedPropertyType.Color;
			return true;
		}
		if (type == typeof(LayerMask)) {
			propertyType = SerializedPropertyType.LayerMask;
			return true;
		}
		if (type.IsEnum) {
			propertyType = SerializedPropertyType.Enum;
			return true;
		}
		if (type == typeof(Vector2)) {
			propertyType = SerializedPropertyType.Vector2;
			return true;
		}
		if (type == typeof(Vector3)) {
			propertyType = SerializedPropertyType.Vector3;
			return true;
		}
		if (type == typeof(Rect)) {
			propertyType = SerializedPropertyType.Rect;
			return true;
		}
		// Missing: ArraySize, Character
		if (type == typeof(AnimationCurve)) {
			propertyType = SerializedPropertyType.AnimationCurve;
			return true;
		}
		if (type == typeof(Bounds)) {
			propertyType = SerializedPropertyType.Bounds;
			return true;
		}
		// Missing: Gradient
		if (type.IsClass) {
			propertyType = SerializedPropertyType.ObjectReference;
			return true;
		}

		return false;
	}

	public static Property[] GetProperties(System.Object obj) {
		List<Property> properties = new List<Property>();
		PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.Public |
		                                                 BindingFlags.Instance);
		for (int i = 0; i < infos.Length; i++) {
			if (!(infos[i].CanRead && infos[i].CanWrite)) {
				continue;
			}

			object[] attributes = infos[i].GetCustomAttributes(true);
			bool isExposed = false;
			for (int j = 0; j < attributes.Length; j++) {
				if (attributes[j].GetType() == typeof(ExposePropertyAttribute)){
					isExposed = true;
					break;
				}
			}
			if (!isExposed) {
				continue;
			}

			SerializedPropertyType type = SerializedPropertyType.Generic;
			if (Property.GetType(infos[i], out type)) {
				Property property = new Property(obj, infos[i], type);
				properties.Add(property);
			}
		}
		return properties.ToArray();
	}
}