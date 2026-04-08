using System;
using Figma.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private const string FigmaSettingsUrl = "https://www.figma.com/settings#security-section";

        private TextField _tokenField;
        private Label _tokenStatus;
        private Label _userInfo;
        private VisualElement _authLogin;
        private VisualElement _authAuthorized;
        private VisualElement _mainContent;

        private void SetupAuth()
        {
            _tokenField = rootVisualElement.Q<TextField>("figma-token");
            _tokenStatus = rootVisualElement.Q<Label>("token-status");
            _userInfo = rootVisualElement.Q<Label>("user-info");
            _authLogin = rootVisualElement.Q<VisualElement>("auth-login");
            _authAuthorized = rootVisualElement.Q<VisualElement>("auth-authorized");
            _mainContent = rootVisualElement.Q<VisualElement>("main-content");

            var btnTokenInfo = rootVisualElement.Q<Button>("btn-token-info");
            var btnTogglePassword = rootVisualElement.Q<Button>("btn-toggle-password");
            var tokenHelp = rootVisualElement.Q<VisualElement>("token-help");
            var btnSignOut = rootVisualElement.Q<Button>("btn-sign-out");

            SetEyeIcon(btnTogglePassword, false);
            btnTogglePassword.clicked += () =>
            {
                _tokenField.isPasswordField = !_tokenField.isPasswordField;
                var visible = !_tokenField.isPasswordField;
                SetEyeIcon(btnTogglePassword, visible);
                btnTogglePassword.EnableInClassList("btn-eye--visible", visible);
            };

            btnTokenInfo.clicked += () =>
            {
                var vis = tokenHelp.ClassListContains("token-help--visible");
                tokenHelp.EnableInClassList("token-help--visible", !vis);
            };

            btnSignOut.clicked += SignOut;

            _tokenField.RegisterValueChangedCallback(evt =>
            {
                var token = evt.newValue?.Trim();
                if (token != evt.newValue)
                    _tokenField.SetValueWithoutNotify(token);

                _tokenStorage.SaveToken(token);

                if (!string.IsNullOrWhiteSpace(token))
                    ValidateToken(token);
                else
                    SetAuthState(false, null);
            });

            var savedToken = _tokenStorage.LoadToken()?.Trim();
            if (!string.IsNullOrEmpty(savedToken))
            {
                _tokenField.SetValueWithoutNotify(savedToken);
                ValidateToken(savedToken);
            }
        }
        
        private void SetupHelpPanel()
        {
            var btnOpenFigma = rootVisualElement.Q<Button>("btn-open-figma");
            btnOpenFigma.clicked += () => Application.OpenURL(FigmaSettingsUrl);

            var helpImage = rootVisualElement.Q<Image>("token-help-image");
            var tipGuids = AssetDatabase.FindAssets("personal_access_token_tip_image t:Texture2D");
            if (tipGuids.Length > 0)
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(tipGuids[0]));
                helpImage.image = tex;
            }
            else
            {
                helpImage.style.display = DisplayStyle.None;
            }
        }

        private async void ValidateToken(string token)
        {
            SetTokenStatus("loading", "Validating...");

            var ct = ResetCancellation();

            try
            {
                using var client = new FigmaApiClient(token);
                var user = await client.GetCurrentUserAsync(ct);
                SetTokenStatus("ok", null);
                SetAuthState(true, $"{user.handle} ({user.email})");
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogWarning($"[FigmaAutoLayout] Token validation failed: {e.Message}");
                SetTokenStatus("error", "Invalid token — check token or scopes");
                SetAuthState(false, null);
            }
        }

        private void SignOut()
        {
            _tokenStorage.SaveToken("");
            _tokenField.SetValueWithoutNotify("");
            
            SetTokenStatus(null, null);
            SetAuthState(false, null);
            ResetFileBrowser();
        }

        private void SetAuthState(bool authorized, string userName)
        {
            _authLogin.EnableInClassList("auth-login--hidden", authorized);
            _authAuthorized.EnableInClassList("auth-authorized--visible", authorized);
            _mainContent.EnableInClassList("main-content--visible", authorized);
            
            _userInfo.text = authorized ? $"Authorized as {userName}" : "";
        }

        private void SetTokenStatus(string state, string message)
        {
            if (_tokenStatus == null) 
                return;
            
            _tokenStatus.text = message ?? "";
            
            _tokenStatus.EnableInClassList("token-status--ok", state == "ok");
            _tokenStatus.EnableInClassList("token-status--error", state == "error");
            _tokenStatus.EnableInClassList("token-status--loading", state == "loading");
        }

        private static void SetEyeIcon(Button button, bool visible)
        {
            var iconName = visible ? "d_scenevis_visible_hover" : "d_scenevis_hidden_hover";
            var icon = EditorGUIUtility.IconContent(iconName);
            button.text = "";
            button.style.backgroundImage = icon?.image as Texture2D;
        }
    }
}
