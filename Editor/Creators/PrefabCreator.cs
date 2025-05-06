using Figma;
using Figma.Creators;
using Figma.Objects;
using Figma.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Figma.Creators
{
    public class PrefabCreator
    {
        private readonly int _layerId;
        private readonly string _prefabsPath;
        private readonly ComponentList _componentList;
        private readonly CreatorsList _creators;

        public PrefabCreator(CreatorsList creators, int layerId, string prefabsPath, ComponentList componentList)
        {
	        _creators = creators;
            _layerId = layerId;
            _prefabsPath = prefabsPath;
            _componentList = componentList;
        }

        public void CreatePrefabFromFigmaFile(FigmaFile file, int selectedPage, int selectedFrame)
        {
            var page = file.document.children[selectedPage];
            var frame = page.children[selectedFrame];

            _componentList.Clean();

            switch (frame.type)
            {
                case FigmaObjectType.COMPONENT_SET:
                    CreateVariants(frame);
                    break;
                case FigmaObjectType.FRAME:
                case FigmaObjectType.COMPONENT:
                case FigmaObjectType.INSTANCE:
                    var prefabName = FormatHelper.FormatPrefabName(frame.name);
                    CreatePrefab(frame, prefabName);
                    break;
                default:
                    Debug.LogError("This must be component set or frame or component or instance, please, choose another object!");
                    break;
            }
        }

        private GameObject CreatePrefab(FigmaObject frame, string prefabName)
        {
            var baseLayer = new GameObject
            {
                name = frame.name,
                layer = _layerId
            };

            var frameRect = baseLayer.AddComponent<RectTransform>();
            frameRect.pivot = new Vector2(0f, 1f);
            frameRect.anchorMin = new Vector2(0f, 1f);
            frameRect.anchorMax = new Vector2(0f, 1f);
            frameRect.anchoredPosition = Vector2.zero;
            if (frame.absoluteBoundingBox.width != null && frame.absoluteBoundingBox.height != null)
                frameRect.sizeDelta = new Vector2(frame.absoluteBoundingBox.width.Value, frame.absoluteBoundingBox.height.Value);

            if (ColorHelper.NeedAddImage(frame.fills))
            {
                var frameImage = baseLayer.AddComponent<Image>();
                frameImage.color = ColorHelper.CalculateColor(frame.fills);
            }

            if (frame.children != null)
                foreach (var child in frame.children)
                {
                    CreateGameObject(child, frame, baseLayer.GetComponent<RectTransform>());
                }

            return CreatePrefab(baseLayer, prefabName);
        }

        private void CreateVariants(FigmaObject componentSet)
        {
            if (componentSet.children == null || componentSet.children.Length == 0)
                return;

            var originPrefabName = FormatHelper.FormatPrefabName(componentSet.name);
            var originPrefab = CreatePrefab(componentSet.children[0], originPrefabName);

            foreach (var child in componentSet.children)
            {
                CreateVariant(originPrefab, child);
            }
        }

        private void CreateVariant(GameObject originPrefab, FigmaObject variantFigmaObject)
        {
            var objSource = (GameObject)PrefabUtility.InstantiatePrefab(originPrefab);
            var variantPrefabName = FormatHelper.FormatVariantPrefabName(variantFigmaObject.name);
            var variantColor = FormatHelper.FormatVariantColor(variantFigmaObject.name);
            var component = _componentList.FindByName(variantPrefabName);
            var variant = component == null ? CreatePrefab(objSource, variantPrefabName) : component.prefab;

            _componentList.Add(variantFigmaObject.id, variant, variantColor);
        }

        private void CreateGameObject(FigmaObject figmaObject, FigmaObject frame, Transform parent)
        {
            GameObject objChild;

            if (figmaObject.type == FigmaObjectType.INSTANCE)
            {
                objChild = InstantiatePrefab(figmaObject);
                if (objChild != null)
                {
                    var rectCreator = new RectTransformCreator();
                    rectCreator.Create(objChild, figmaObject, parent, frame);
                    return;
                }
            }

            objChild = new GameObject
            {
                name = figmaObject.name,
                layer = _layerId,
            };

            objChild.SetActive(figmaObject.visible);

            if (figmaObject.type != FigmaObjectType.DOCUMENT && figmaObject.type != FigmaObjectType.CANVAS)
            {
                foreach (var creator in _creators)
                {
                    creator?.Create(objChild, figmaObject, parent, frame);
                }
            }

            if (figmaObject.children == null)
                return;

            foreach (var child in figmaObject.children)
            {
                CreateGameObject(child, frame, objChild.GetComponent<RectTransform>());
            }

            if (figmaObject.type == FigmaObjectType.COMPONENT || figmaObject.type == FigmaObjectType.INSTANCE)
            {
                var rect = objChild.GetComponent<RectTransform>();
                var pos = rect.anchoredPosition;
                var size = rect.sizeDelta;

                var prefabName = FormatHelper.FormatPrefabName(figmaObject.name);
                var prefab = CreatePrefab(objChild, prefabName);
                if (prefab == null)
                    return;

                var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.SetParent(parent);

                rect = instance.GetComponent<RectTransform>();
                rect.anchoredPosition = pos;
                rect.sizeDelta = size;

                PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());
                _componentList.Add(figmaObject.id, prefab, string.Empty);
            }
        }

        private GameObject InstantiatePrefab(FigmaObject figmaObject)
        {
            // try find prefab in component list by id
            var component = _componentList.Find(figmaObject.componentId);
            if (component != null)
            {
                var instance = (GameObject) PrefabUtility.InstantiatePrefab(component.prefab);
                PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());
                return instance;
            }

            // try find prefab by name in project
            var path = FormatHelper.FormatPath(_prefabsPath);
            var prefabName = FormatHelper.FormatPrefabName(figmaObject.name);
            var guids = AssetDatabase.FindAssets($"t:Prefab {prefabName}", new[] {path});
            foreach (var guid in guids)
            {
                var prefab = (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject));
                if (prefab != null && prefab.name == prefabName)
                {
                    var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());
                    return instance;
                }
            }

            // create prefab in another way
            return null;
        }

        private GameObject CreatePrefab(GameObject obj, string prefabName)
        {
            var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(_prefabsPath + prefabName + ".prefab");
            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, uniqueAssetPath);
            Object.DestroyImmediate(obj);
            return prefab;
        }
    }
}
