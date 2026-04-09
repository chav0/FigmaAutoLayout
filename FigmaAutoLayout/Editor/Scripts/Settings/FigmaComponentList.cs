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
        
        public GameObject FindPrefab(string key, string name = null)
        {
            var component = FindComponent(key, name);
            if (component != null)
                return component.prefab;
            
            foreach (var comp in components)
            {
                var variant = comp.FindVariant(key, name);
                if (variant == null)
                    continue;

                return variant.prefab != null ? variant.prefab : comp.prefab;
            }

            return null;
        }

        public FigmaComponent FindComponent(string key, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var byName = components.FirstOrDefault(c => c.name == name);
                if (byName != null)
                    return byName;
            }

            if (!string.IsNullOrEmpty(key))
            {
                var byKey = components.FirstOrDefault(c => c.id == key);
                if (byKey != null)
                    return byKey;
            }

            return null;
        }

        public void AddComponent(string key, string name, GameObject prefab)
        {
            var existing = FindComponent(key, name);
            if (existing != null)
            {
                existing.id = key;
                existing.name = name;
                if (prefab != null)
                    existing.prefab = prefab;
                return;
            }

            components.Add(new FigmaComponent(key, name, prefab));
        }

        public void AddVariant(string componentKey, string componentName,
            string variantKey, string variantName, GameObject variantPrefab)
        {
            var component = FindComponent(componentKey, componentName);
            if (component == null)
            {
                component = new FigmaComponent(componentKey, componentName, null);
                components.Add(component);
            }

            component.AddVariant(variantKey, variantName, variantPrefab);
        }

        public void Clean()
        {
            for (var i = components.Count - 1; i >= 0; i--)
            {
                if (components[i].prefab == null && components[i].variants.Count == 0)
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
        public List<FigmaComponentVariant> variants = new();

        public FigmaComponent(string id, string name, GameObject prefab)
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
        }

        public FigmaComponentVariant FindVariant(string key, string variantName = null)
        {
            if (!string.IsNullOrEmpty(variantName))
            {
                var byName = variants.FirstOrDefault(v => v.name == variantName);
                if (byName != null)
                    return byName;
            }

            if (!string.IsNullOrEmpty(key))
            {
                var byKey = variants.FirstOrDefault(v => v.id == key);
                if (byKey != null)
                    return byKey;
            }

            return null;
        }

        public void AddVariant(string key, string variantName, GameObject variantPrefab)
        {
            var existing = FindVariant(key, variantName);
            if (existing != null)
            {
                existing.id = key;
                existing.name = variantName;
                if (variantPrefab != null)
                    existing.prefab = variantPrefab;
                return;
            }

            variants.Add(new FigmaComponentVariant(key, variantName, variantPrefab));
        }
    }

    [Serializable]
    public class FigmaComponentVariant
    {
        public string name;
        public string id;
        public GameObject prefab;

        public FigmaComponentVariant(string id, string name, GameObject prefab)
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
        }
    }
}
