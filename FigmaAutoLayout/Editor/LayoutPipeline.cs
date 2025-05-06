using System;
using System.Collections.Generic;
using System.Linq;
using Figma.Creators;
using Figma.Utils;
using UnityEditor;
using UnityEngine;

namespace Figma
{
	[CreateAssetMenu(menuName = "Figma/LayoutPipeline")]
	public class LayoutPipeline : ScriptableObject
	{
		[SerializeField] private CreatorsList creators = new()
		{
			Creators = new List<CreatorBase>
			{
				new TextCreator(),
				new RectTransformCreator(),
				new ImageCreator(),
				new VerticalGroupCreator(),
				new HorizontalGroupCreator(),
				new ContentSizeFitterCreator()
			} 
		};

		public CreatorsList Creators => creators;
	}

	[CustomPropertyDrawer(typeof(CreatorsList), true)]
	public class CreatorsListEditor : ListPropertyDrawer
	{
		private Type[] _types;

		protected override string PropertyName => "Creators";
		protected override Type[] Types
		{
			get
			{
				if (_types != null)
					return _types;

				var unityObjectType = typeof(UnityEngine.Object);
				var baseType = typeof(CreatorBase);

				_types = AppDomain.CurrentDomain
					.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(type => !type.IsAbstract && type.IsSerializable)
					.Where(type => baseType.IsAssignableFrom(type) && !unityObjectType.IsAssignableFrom(type))
					.Prepend(default)
					.ToArray();

				return _types;
			}
		}
	}

}
