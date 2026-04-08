using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figma
{
    [Serializable]
    public class FigmaComponentList
    {
        [SerializeField] private List<FigmaComponent> components = new();

        public FigmaComponent Find(string id, string name = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var byId = components.FirstOrDefault(c => c.id == id);
                if (byId != null)
                    return byId;
            }
            
            if (!string.IsNullOrEmpty(name))
            {
                var byName = FindByName(name);
                if (byName != null)
                    return byName;
            }

            return null;
        }

        public FigmaComponent FindByName(string componentName)
        {
            return components.FirstOrDefault(c =>
                c.prefab != null && c.name == componentName);
        }

        public void Add(string id, string name, GameObject prefab, string color, bool isMainComponent = false)
        {
            var existing = components.FirstOrDefault(c => c.id == id);
            if (existing != null)
            {
                if (isMainComponent || existing.prefab == null)
                {
                    existing.name = name;
                    existing.prefab = prefab;
                    existing.color = color;
                }
            }
            else
            {
                components.Add(new FigmaComponent(id, name, prefab, color));
            }
        }
        
        public void Clean()
        {
            for (var i = components.Count - 1; i >= 0; i--)
            {
                if (components[i].prefab == null)
                    components.RemoveAt(i);
            }
        }
    }

    [Serializable]
    public class FigmaComponent
    {
        public string name;
        public string id;
        public GameObject prefab;
        public string color;

        public FigmaComponent(string id, string name, GameObject prefab, string color)
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
            this.color = color;
        }
    }
}
