using Blobler.Objects;
using UnityEngine;

namespace Blobler.Creators
{
    public interface ICreator
    {
        void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame); 
    }
}
