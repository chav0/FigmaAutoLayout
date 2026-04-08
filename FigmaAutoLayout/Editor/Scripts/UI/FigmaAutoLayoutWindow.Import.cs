using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Figma.Objects;
using Figma.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private const string RecentFilesKey = "FigmaAutoLayout_RecentFiles";
        private const int MaxRecentFiles = 3;

        private FigmaFile _parsedFile;
        private FigmaApiClient _client;
        private string _fileKey;
        private int _selectedPage;
        private int _selectedFrame;

        private VisualElement _fileBrowser;
        private VisualElement _pagesList;
        private VisualElement _framesList;
        private TextField _frameSearch;
        private Image _framePreview;
        private Label _previewLabel;
        private Label _previewLoading;
        private Label _importStatus;
        private Button _btnParse;

        private string _previewNodeId;

        private TextField _fileUrlField;
        private VisualElement _recentDropdown;

        private void SetupImport()
        {
            _fileUrlField = rootVisualElement.Q<TextField>("file-url");
            _btnParse = rootVisualElement.Q<Button>("btn-parse");
            _importStatus = rootVisualElement.Q<Label>("import-status");
            _fileBrowser = rootVisualElement.Q<VisualElement>("file-browser");
            _pagesList = rootVisualElement.Q<VisualElement>("pages-list");
            _framesList = rootVisualElement.Q<VisualElement>("frames-list");
            _frameSearch = rootVisualElement.Q<TextField>("frame-search");
            _framePreview = rootVisualElement.Q<Image>("frame-preview");
            _previewLabel = rootVisualElement.Q<Label>("preview-label");
            _previewLoading = rootVisualElement.Q<Label>("preview-loading");

            _recentDropdown = new VisualElement();
            _recentDropdown.AddToClassList("recent-dropdown");
            rootVisualElement.Add(_recentDropdown);

            _fileUrlField.RegisterValueChangedCallback(evt =>
            {
                _btnParse.EnableInClassList("btn-parse--hidden", string.IsNullOrWhiteSpace(evt.newValue));
            });

            _fileUrlField.RegisterCallback<FocusInEvent>(_ => ShowRecentDropdown());
            _fileUrlField.RegisterCallback<FocusOutEvent>(_ =>
            {
                _fileUrlField.schedule.Execute(() => HideRecentDropdown()).ExecuteLater(150);
            });

            _btnParse.clicked += () => _ = StartImport(_fileUrlField.value);
            _frameSearch.RegisterValueChangedCallback(_ => RebuildFramesList());
        }

        private List<string> LoadRecentFiles()
        {
            var json = EditorPrefs.GetString(RecentFilesKey, "[]");

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private void SaveRecentFile(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            var recent = LoadRecentFiles();
            recent.Remove(url);
            recent.Insert(0, url);

            if (recent.Count > MaxRecentFiles)
                recent.RemoveRange(MaxRecentFiles, recent.Count - MaxRecentFiles);

            EditorPrefs.SetString(RecentFilesKey, JsonConvert.SerializeObject(recent));
        }

        private void ShowRecentDropdown()
        {
            var recent = LoadRecentFiles();
            _recentDropdown.Clear();

            if (recent.Count == 0) return;

            foreach (var url in recent)
            {
                var item = new VisualElement();
                item.AddToClassList("recent-dropdown__item");
                item.Add(new Label(url));
                item.RegisterCallback<ClickEvent>(_ =>
                {
                    _fileUrlField.value = url;
                    HideRecentDropdown();
                });
                _recentDropdown.Add(item);
            }

            PositionDropdown();
            _recentDropdown.EnableInClassList("recent-dropdown--visible", true);
        }

        private void PositionDropdown()
        {
            var textInput = _fileUrlField.Q("unity-text-input");
            var inputRect = textInput.worldBound;
            var rootRect = rootVisualElement.worldBound;

            _recentDropdown.style.top = inputRect.yMax - rootRect.y;
            _recentDropdown.style.left = inputRect.x - rootRect.x;
            _recentDropdown.style.width = inputRect.width;
        }

        private void HideRecentDropdown()
        {
            _recentDropdown.EnableInClassList("recent-dropdown--visible", false);
        }

        private async Task StartImport(string fileUrl)
        {
            try
            {
                _fileKey = FigmaUrlHelper.ExtractFileKey(fileUrl);
            }
            catch (Exception e)
            {
                SetImportStatus("error", e.Message);
                return;
            }

            SetImportStatus("loading", "Loading file from Figma...");
            _btnParse.SetEnabled(false);
            ResetFileBrowser();

            var ct = ResetCancellation();

            var token = _tokenStorage.LoadToken()?.Trim();
            _client?.Dispose();
            _client = new FigmaApiClient(token);

            try
            {
                _parsedFile = await _client.GetFileAsync(_fileKey, ct);
                SetImportStatus();
                SaveRecentFile(_fileUrlField.value);
                ShowFileBrowser();
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogWarning($"[FigmaAutoLayout] Import failed: {e.Message}");
                SetImportStatus("error", $"Import failed: {e.Message}");
            }
            finally
            {
                _btnParse.SetEnabled(true);
            }
        }

        private void ShowFileBrowser()
        {
            _fileBrowser.EnableInClassList("file-browser--visible", true);

            var pages = _parsedFile.document.children;
            _pagesList.Clear();

            for (var i = 0; i < pages.Length; i++)
            {
                var idx = i;
                var item = CreateBrowserItem(pages[i].name);
                item.RegisterCallback<ClickEvent>(_ => SelectPage(idx));
                _pagesList.Add(item);
            }

            SelectPage(0);
        }

        private void SelectPage(int index)
        {
            _selectedPage = index;

            for (var i = 0; i < _pagesList.childCount; i++)
                _pagesList[i].EnableInClassList("browser-item--selected", i == index);

            _selectedFrame = 0;
            _frameSearch.SetValueWithoutNotify("");
            RebuildFramesList();
        }

        private void RebuildFramesList()
        {
            _framesList.Clear();

            var children = _parsedFile.document.children[_selectedPage].children;
            if (children == null || children.Length == 0) return;

            var search = _frameSearch.value?.ToLowerInvariant() ?? "";

            for (var i = 0; i < children.Length; i++)
            {
                var name = children[i].name;
                if (!string.IsNullOrEmpty(search) && !name.ToLowerInvariant().Contains(search))
                    continue;

                var idx = i;
                var displayName = name.Length > 30 ? name.Substring(0, 30) + "\u2026" : name;
                var item = CreateBrowserItem(displayName);
                if (idx == _selectedFrame)
                    item.AddToClassList("browser-item--selected");
                item.RegisterCallback<ClickEvent>(_ => SelectFrame(idx));
                _framesList.Add(item);
            }

            _ = RequestThumbnail();
        }

        private void SelectFrame(int index)
        {
            _selectedFrame = index;

            foreach (var child in _framesList.Children())
                child.RemoveFromClassList("browser-item--selected");

            foreach (var child in _framesList.Children())
            {
                if (child.userData is int idx && idx == index)
                    child.AddToClassList("browser-item--selected");
            }

            RebuildFramesList();
        }

        private VisualElement CreateBrowserItem(string text)
        {
            var item = new VisualElement();
            item.AddToClassList("browser-item");
            item.Add(new Label(text));
            return item;
        }

        private async Task RequestThumbnail()
        {
            var children = _parsedFile.document.children[_selectedPage].children;
            if (children == null || _selectedFrame >= children.Length) return;

            var nodeId = children[_selectedFrame].id;
            if (nodeId == _previewNodeId) return;

            _previewNodeId = nodeId;
            _framePreview.image = null;
            _framePreview.EnableInClassList("frame-preview--visible", false);
            _previewLabel.style.display = DisplayStyle.None;
            _previewLoading.text = "Loading preview...";
            _previewLoading.EnableInClassList("import-status--visible", true);

            try
            {
                var bytes = await _client.GetNodeThumbnailAsync(_fileKey, nodeId, ct: _cts?.Token ?? CancellationToken.None);
                _previewLoading.EnableInClassList("import-status--visible", false);

                var tex = FigmaTextureHelper.CreateFromBytes(bytes);
                if (tex != null)
                {
                    _framePreview.image = tex;
                    _framePreview.EnableInClassList("frame-preview--visible", true);
                    _previewLabel.style.display = DisplayStyle.Flex;
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _previewLoading.EnableInClassList("import-status--visible", false);
                Debug.LogWarning($"[FigmaAutoLayout] Preview failed: {e.Message}");
            }
        }

        private void SetImportStatus(string state = null, string message = null)
        {
            if (_importStatus == null)
                return;

            _importStatus.text = message ?? "";
            _importStatus.EnableInClassList("import-status--visible", state != null);
            _importStatus.EnableInClassList("import-status--error", state == "error");
        }

        private void ResetFileBrowser()
        {
            _parsedFile = null;
            _previewNodeId = null;
            _fileBrowser?.EnableInClassList("file-browser--visible", false);
        }
    }
}
