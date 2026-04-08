using Figma.Exporters;
using UnityEditor;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private void RefreshSettingsUI()
        {
            _prefabExporter = new PrefabExporter(_settings);
            _spriteExporter = new SpriteExporter(_settings);

            RefreshSpritesUI();
            RefreshPrefabsUI();
        }
        
        private void SaveSettings()
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssetIfDirty(_settings);
        }
    }
}
