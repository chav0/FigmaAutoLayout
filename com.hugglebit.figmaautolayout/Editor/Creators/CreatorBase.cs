using System;
using Figma.Objects;
using UnityEngine;

namespace Figma.Creators
{
	[Serializable]
	public abstract class CreatorBase
	{
		public abstract void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame); 
	}
}
