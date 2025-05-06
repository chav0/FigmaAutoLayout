using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Figma.Creators
{
	[Serializable]
	public class CreatorsList : IEnumerable<CreatorBase>
	{
		[SerializeReference]
		public List<CreatorBase> Creators;
		public IEnumerator<CreatorBase> GetEnumerator() => Creators.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
