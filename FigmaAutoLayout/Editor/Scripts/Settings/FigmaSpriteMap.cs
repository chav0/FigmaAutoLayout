using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figma
{
    [Serializable]
    public class FigmaSpriteMap
    {
        [SerializeField] private List<FigmaIconEntry> entries = new();

        public Sprite Find(string nameOrId)
        {
            if (string.IsNullOrEmpty(nameOrId))
                return null;

            return entries.FirstOrDefault(e =>
                e.sprite != null && (e.name == nameOrId || e.id == nameOrId))?.sprite;
        }

        public void Add(string name, Sprite sprite, string id = null)
        {
            var existing = entries.FirstOrDefault(e => e.name == name);
            if (existing != null)
            {
                existing.sprite = sprite;
                if (!string.IsNullOrEmpty(id))
                    existing.id = id;
            }
            else
            {
                entries.Add(new FigmaIconEntry(name, sprite, id));
            }
        }
    }

    [Serializable]
    public class FigmaIconEntry
    {
        public string name;
        public string id;
        public Sprite sprite;

        public FigmaIconEntry(string name, Sprite sprite, string id = null)
        {
            this.name = name;
            this.sprite = sprite;
            this.id = id;
        }
    }
}
