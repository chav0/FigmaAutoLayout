using System;
using System.Linq;
using Blobler.Creators;
using UnityEditor;
using UnityEngine;

namespace Blobler
{
	[CreateAssetMenu(menuName = "Blobler/LayoutPipeline")]
	public class LayoutPipeline : ScriptableObject
	{
		[SerializeField] private CreatorsList creators;

		public CreatorsList Creators => creators;
	}

	[CustomPropertyDrawer(typeof(CreatorsList), true)]
	public class InteractiveElementConditionsEditor : ListPropertyDrawer
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
