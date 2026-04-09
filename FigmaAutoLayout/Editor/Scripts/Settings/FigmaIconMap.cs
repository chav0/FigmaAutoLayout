using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figma
{
    [Serializable]
    public class FigmaIconMap
    {
        [SerializeField] private List<FigmaIconEntry> entries = new();

        public Sprite Find(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return entries.FirstOrDefault(e => e.name == name && e.sprite != null)?.sprite;
        }

        public void Add(string name, Sprite sprite)
        {
            var existing = entries.FirstOrDefault(e => e.name == name);
            if (existing != null)
                existing.sprite = sprite;
            else
                entries.Add(new FigmaIconEntry(name, sprite));
        }
    }

    [Serializable]
    public class FigmaIconEntry
    {
        public string name;
        public Sprite sprite;

        public FigmaIconEntry(string name, Sprite sprite)
        {
            this.name = name;
            this.sprite = sprite;
        }
    }
}
