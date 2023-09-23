using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Blobler
{
	public abstract class ListPropertyDrawer : PropertyDrawer
	{
		private SerializedProperty _serializedProperty;
		private ReorderableList _list;

		protected abstract string PropertyName { get; }
		protected abstract Type[] Types { get; }

		private void Init(SerializedProperty property)
		{
			if (_list != null)
				return;

			var array = property.FindPropertyRelative(PropertyName);

			_serializedProperty = property;
			_list = new ReorderableList(property.serializedObject, array);

			_list.drawElementCallback += DrawElement;
			_list.elementHeightCallback += GetStepHeight;
			_list.onAddDropdownCallback += OnClickToAddNew;
			_list.drawHeaderCallback += DrawHeader;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init(property);
			_list.DoList(position);
			property.serializedObject.ApplyModifiedProperties();
		}
		
		private void DrawHeader(Rect rect) => GUI.Label(rect, PropertyName);

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.xMin += 10.0f;
			var element = _list.serializedProperty.GetArrayElementAtIndex(index);
			var elementName = ObjectNames.NicifyVariableName(element.managedReferenceValue.GetType().Name);
			element.isExpanded = true;
			EditorGUI.PropertyField(rect, element, new GUIContent(elementName), element.isExpanded);
		}

		private float GetStepHeight(int index)
		{
			var element = _list.serializedProperty.GetArrayElementAtIndex(index);
			return EditorGUI.GetPropertyHeight(element, GUIContent.none, element.isExpanded);
		}

		private void OnClickToAddNew(Rect rect, ReorderableList list)
		{
			var menu = new GenericMenu();
			var serializedProperty = list.serializedProperty;
			for (int i = 1, length = Types.Length; i < length; ++i)
			{
				var type = Types[i];
				var showItem = true; 

				for (var j = 0; j < serializedProperty.arraySize; j++)
				{
					var existingType = serializedProperty.GetArrayElementAtIndex(j).managedReferenceValue.GetType();
					if (type == existingType)
					{
						showItem = false;
						break;
					}
				}
				
				if (showItem)
					menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => AddItem(type));
			}
			
			menu.ShowAsContext();
		}

		private void AddItem(Type type)
		{
			_serializedProperty.serializedObject.Update();

			var list = _list.serializedProperty;
			var index = list.arraySize;

			list.InsertArrayElementAtIndex(index);
			list.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(type);

			_serializedProperty.serializedObject.ApplyModifiedProperties();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Init(property);
			return _list.GetHeight();
		}
	}
}
