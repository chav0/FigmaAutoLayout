using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blobler
{
    [CreateAssetMenu(menuName = "Blobler/ComponentList")]
    public class ComponentList : ScriptableObject
    {
        [SerializeField] private List<Component> components;

        public Component Find(string id)
        {
            return components.FirstOrDefault(component => component.id == id);
        }
        
        public Component FindByName(string prefabName)
        {
            return components.FirstOrDefault(component => component.prefab.name == prefabName);
        }

        public void Clean()
        {
            for (var i = components.Count - 1; i >= 0; i--)
            {
                var component = components[i]; 
                if(component.prefab == null)
                    components.RemoveAt(i);
            }
        }

        public void Add(string id, GameObject prefab, string color)
        {
            if (components.Count(x => x.id == id) == 0)
            {
                var component = new Component(id, prefab, color);
                components.Add(component);
            }
            else
            {
                var component = Find(id);
                component.prefab = prefab;
                component.color = color; 
            }
        }
    }

    [Serializable]
    public class Component
    {
        public string id;
        public GameObject prefab;
        public string color;

        public Component(string id, GameObject prefab, string color)
        {
            this.id = id;
            this.prefab = prefab;
            this.color = color; 
        }
    }
}