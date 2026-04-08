using System.Collections.Generic;
using Figma.Objects;
using Figma.PipelineSteps;
using Figma.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Figma.Exporters
{
    public class PrefabExporter
    {
        private readonly string _prefabsPath;
        private readonly FigmaComponentList _componentList;
        private List<FigmaPreLayoutPipelineStepBase> _preSteps;
        private List<FigmaLayoutPipelineObjectStepBase> _objectSteps;
        private List<FigmaPostLayoutPipelineStepBase> _postSteps;

        private LayerMask UILayer => LayerMask.NameToLayer("UI");

        public PrefabExporter(FigmaAutoLayoutSettings settings)
        {
            _prefabsPath = settings.PrefabFolderPath;
            _componentList = settings.ComponentList;
        }

        public void SetPipeline(FigmaLayoutPipelineProfile profile)
        {
            _preSteps = profile.PreLayoutSteps;
            _objectSteps = profile.ObjectLayoutSteps;
            _postSteps = profile.PostLayoutSteps;
        }

        public void Export(FigmaFile file, int selectedPage, int selectedFrame)
        {
            var page = file.document.children[selectedPage];
            var frame = page.children[selectedFrame];

            _componentList.Clean();

            var preContext = new PreLayoutContext(frame);

            if (_preSteps != null)
                foreach (var step in _preSteps)
                    step?.Execute(preContext);

            GameObject result = null;

            switch (frame.type)
            {
                case FigmaObjectType.COMPONENT_SET:
                    result = CreateVariants(frame);
                    break;
                case FigmaObjectType.FRAME:
                case FigmaObjectType.COMPONENT:
                case FigmaObjectType.INSTANCE:
                    var prefabName = FigmaAssetPathHelper.SanitizeName(frame.name);
                    result = CreatePrefab(frame, prefabName, preContext.RootGameObject);
                    break;
                default:
                    Debug.LogError("[FigmaAutoLayout] Unsupported frame type. Select a Frame, Component, or Component Set.");
                    break;
            }

            if (result != null)
            {
                var postContext = new PostLayoutContext(result, frame);
                if (_postSteps != null)
                {
                    foreach (var step in _postSteps)
                        step?.Execute(postContext);
                }

                EditorGUIUtility.PingObject(result);
            }
        }

        private GameObject CreatePrefab(FigmaObject frame, string prefabName, GameObject rootGameObject = null)
        {
            var baseLayer = rootGameObject != null ? rootGameObject : new GameObject();
            baseLayer.name = frame.name;
            baseLayer.layer = UILayer;

            var frameRect = baseLayer.GetComponent<RectTransform>();
            if (frameRect == null)
                frameRect = baseLayer.AddComponent<RectTransform>();
            
            frameRect.pivot = new Vector2(0f, 1f);
            frameRect.anchorMin = new Vector2(0f, 1f);
            frameRect.anchorMax = new Vector2(0f, 1f);
            frameRect.anchoredPosition = Vector2.zero;
            
            if (frame.absoluteBoundingBox.width != null && frame.absoluteBoundingBox.height != null)
                frameRect.sizeDelta = new Vector2(frame.absoluteBoundingBox.width.Value, frame.absoluteBoundingBox.height.Value);

            if (FigmaColorHelper.NeedAddImage(frame.fills))
            {
                var frameImage = baseLayer.GetComponent<Image>();
                if (frameImage == null)
                    frameImage = baseLayer.AddComponent<Image>();
                frameImage.color = FigmaColorHelper.CalculateColor(frame.fills);
            }

            if (frame.children != null)
            {
                foreach (var child in frame.children)
                {
                    CreateGameObject(child, frame, baseLayer.GetComponent<RectTransform>());
                }
            }

            return SavePrefab(baseLayer, prefabName);
        }

        private GameObject CreateVariants(FigmaObject componentSet)
        {
            if (componentSet.children == null || componentSet.children.Length == 0)
                return null;

            var originPrefabName = FigmaAssetPathHelper.SanitizeName(componentSet.name);
            var originPrefab = CreatePrefab(componentSet.children[0], originPrefabName);

            foreach (var child in componentSet.children)
            {
                CreateVariant(originPrefab, child);
            }

            return originPrefab;
        }

        private void CreateVariant(GameObject originPrefab, FigmaObject variantFigmaObject)
        {
            var objSource = (GameObject)PrefabUtility.InstantiatePrefab(originPrefab);
            var variantPrefabName = FigmaAssetPathHelper.ExtractVariantPrefabName(variantFigmaObject.name);
            var variantColor = FigmaAssetPathHelper.ExtractVariantColor(variantFigmaObject.name);
            
            var component = _componentList.FindByName(variantPrefabName);
            var variant = component == null ? SavePrefab(objSource, variantPrefabName) : component.prefab;

            _componentList.Add(variantFigmaObject.id, variantPrefabName, variant, variantColor, true);
        }

        private void CreateGameObject(FigmaObject figmaObject, FigmaObject rootFrame, Transform parent)
        {
            GameObject objChild;

            if (figmaObject.type == FigmaObjectType.INSTANCE)
            {
                objChild = InstantiatePrefab(figmaObject);
                if (objChild != null)
                {
                    var rectStep = new RectTransformPipelineStep();
                    rectStep.Execute(new ObjectLayoutContext(objChild, figmaObject, parent, rootFrame));
                    return;
                }
            }

            objChild = new GameObject
            {
                name = figmaObject.name,
                layer = UILayer
            };

            objChild.SetActive(figmaObject.visible);

            if (figmaObject.type != FigmaObjectType.DOCUMENT && figmaObject.type != FigmaObjectType.CANVAS)
            {
                var ctx = new ObjectLayoutContext(objChild, figmaObject, parent, rootFrame);
                
                foreach (var step in _objectSteps)
                    step?.Execute(ctx);
            }

            if (figmaObject.children == null)
                return;

            foreach (var child in figmaObject.children)
            {
                CreateGameObject(child, rootFrame, objChild.GetComponent<RectTransform>());
            }

            if (figmaObject.type == FigmaObjectType.COMPONENT || figmaObject.type == FigmaObjectType.INSTANCE)
            {
                var rect = objChild.GetComponent<RectTransform>();
                var pos = rect.anchoredPosition;
                var size = rect.sizeDelta;

                var prefabName = FigmaAssetPathHelper.SanitizeName(figmaObject.name);
                var prefab = SavePrefab(objChild, prefabName);
                if (prefab == null)
                    return;

                var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.SetParent(parent);

                rect = instance.GetComponent<RectTransform>();
                rect.anchoredPosition = pos;
                rect.sizeDelta = size;

                PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());

                var isMain = figmaObject.type == FigmaObjectType.COMPONENT;
                var componentKey = isMain ? figmaObject.id : figmaObject.componentId;
                _componentList.Add(componentKey, prefabName, prefab, string.Empty, isMain);
            }
        }

        private GameObject InstantiatePrefab(FigmaObject figmaObject)
        {
            var componentName = FigmaAssetPathHelper.SanitizeName(figmaObject.name);
            var component = _componentList.Find(figmaObject.componentId, componentName);
            if (component != null)
            {
                var instance = (GameObject) PrefabUtility.InstantiatePrefab(component.prefab);
                PrefabUtility.RecordPrefabInstancePropertyModifications(instance.GetComponent<Transform>());
                return instance;
            }

            var path = _prefabsPath.TrimEnd('/');
            var prefabName = FigmaAssetPathHelper.SanitizeName(figmaObject.name);
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

            return null;
        }

        private GameObject SavePrefab(GameObject obj, string prefabName)
        {
            var assetPath = FigmaAssetPathHelper.BuildAssetPath(_prefabsPath, prefabName, "prefab");
            var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, uniqueAssetPath);
            Object.DestroyImmediate(obj);
            return prefab;
        }
    }
}
