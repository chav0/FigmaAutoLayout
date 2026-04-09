using System.Collections.Generic;
using System.Linq;
using Figma.Objects;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private TreeView _hierarchyTree;
        private VisualElement _hierarchyCol;

        private readonly Dictionary<int, FigmaObject> _treeIdToNode = new();

        public FigmaObject SelectedFigmaObject
        {
            get
            {
                if (_selectedNode != null)
                    return _selectedNode;

                if (_parsedFile == null)
                    return null;

                var children = _parsedFile.document.children[_selectedPage].children;
                return children != null && _selectedFrame < children.Length
                    ? children[_selectedFrame]
                    : null;
            }
        }

        private void SetupHierarchy()
        {
            _hierarchyCol = rootVisualElement.Q<VisualElement>("hierarchy-col");
            _hierarchyTree = rootVisualElement.Q<TreeView>("hierarchy-tree");

            _hierarchyTree.makeItem = () => new Label();
            _hierarchyTree.bindItem = (element, index) =>
            {
                var node = _hierarchyTree.GetItemDataForIndex<FigmaObject>(index);
                var label = (Label)element;
                label.text = node != null ? node.name : "—";
            };

            _hierarchyTree.selectionChanged += OnHierarchySelectionChanged;
            _hierarchyCol.style.display = DisplayStyle.None;
        }

        private void BuildHierarchyTree(FigmaObject frame)
        {
            _treeIdToNode.Clear();
            _selectedNode = null;

            if (frame == null)
            {
                _hierarchyCol.style.display = DisplayStyle.None;
                return;
            }

            var counter = 0;
            var rootItems = new List<TreeViewItemData<FigmaObject>>
            {
                BuildTreeItem(frame, ref counter)
            };

            _hierarchyTree.SetRootItems(rootItems);
            _hierarchyTree.Rebuild();
            _hierarchyCol.style.display = DisplayStyle.Flex;
        }

        private TreeViewItemData<FigmaObject> BuildTreeItem(FigmaObject node, ref int counter)
        {
            var id = counter++;
            _treeIdToNode[id] = node;

            List<TreeViewItemData<FigmaObject>> childItems = null;

            if (node.children is { Length: > 0 })
            {
                childItems = new List<TreeViewItemData<FigmaObject>>(node.children.Length);
                foreach (var child in node.children)
                    childItems.Add(BuildTreeItem(child, ref counter));
            }

            return new TreeViewItemData<FigmaObject>(id, node, childItems);
        }

        private void OnHierarchySelectionChanged(IEnumerable<object> selection)
        {
            var selected = selection.FirstOrDefault();
            if (selected is FigmaObject node)
            {
                _selectedNode = node;
                _ = RequestImage(node.id);
            }
            else
            {
                _selectedNode = null;
            }

            UpdateCreateButtonState();
        }

        private void ClearHierarchy()
        {
            _selectedNode = null;
            _treeIdToNode.Clear();
            _hierarchyCol.style.display = DisplayStyle.None;
        }
    }
}
