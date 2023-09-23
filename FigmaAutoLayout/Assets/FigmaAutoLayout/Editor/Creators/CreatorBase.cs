using System;
using Blobler.Objects;
using UnityEngine;

namespace Blobler.Creators
{
	[Serializable]
	public abstract class CreatorBase
	{
		public abstract void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame); 
	}
}
