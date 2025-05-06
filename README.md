# Figma Auto Layout for Unity

**Figma Auto Layout** is a Unity Editor package that transforms Figma designs into structured Unity UI prefabs. It preserves anchors, hierarchy, naming, and even adds specific components. This tool dramatically speeds up UI layout workflows in Unity.

---

## ✨ Features

* Import Figma frames and pages
* Build UI prefabs using a custom Layout Pipeline
* Download assets from Figma (now only .jpg)

---

## 📆 Installation

### 🔗 Git (recommended)

Edit your Unity project's `Packages/manifest.json` and add:

```json
"com.hugglebit.figmaautolayout": "https://github.com/chav0/FigmaAutoLayout.git?path=FigmaAutoLayout"
```

---

### 🧩 Unity Editor (manual)

You can also install this package directly from the Unity Editor:

1. Open `Window → Package Manager`
2. Click the `+` button → `Add package from Git URL...`
3. Paste this URL:

```
https://github.com/chav0/FigmaAutoLayout.git?path=FigmaAutoLayout
```

---

## 🛠 How to Use

1. Open the tool via `Tools → UI → Figma`
2. Enter your Figma Personal Access Token and File URL
3. Set:

   * UI Prefab path
   * UI Layer ID
   * Layout Pipeline (ScriptableObject)
   * Component List (ScriptableObject)
4. Click `Parse Figma File` to load available pages/frames
5. Select a page and frame, then click `Create Prefab`

---

## 🔐 Authentication

This plugin currently requires a [Figma personal access token](https://www.figma.com/developers/api#access-tokens).

You need to create it in the settings in Figma: Settings → Sequrity → Personal access tokens → Generate new token.

OAuth authentication may be added in future versions.

---

## 📚 Dependencies

This package uses:

* `com.unity.nuget.newtonsoft-json`
* `com.unity.textmeshpro`

They will be installed automatically by Unity Package Manager.

---

## 🐱 Author

Made with love by **Hugglebit**.
Feel free to contribute, suggest features, or report issues at:
[https://github.com/chav0/com.hugglebit.figmaautolayout](https://github.com/chav0/com.hugglebit.figmaautolayout)


<a href="https://www.buymeacoffee.com/alinulken" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>
