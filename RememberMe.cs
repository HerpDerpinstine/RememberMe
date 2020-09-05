using System;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.UI;

namespace RememberMe
{
    public static class BuildInfo
    {
        public const string Name = "RememberMe";
        public const string Author = "Herp Derpinstine";
        public const string Company = "Lava Gang";
        public const string Version = "1.0.0";
        public const string DownloadLink = "https://github.com/HerpDerpinstine/RememberMe";
    }

    public class RememberMe : MelonMod
    {
        private static VRCUiPageAuthentication PageAuthentication = null;
        private static bool ShouldRemember = false;
        private static MethodInfo ValidateTextTargetMethod;

        public override void VRChat_OnUiManagerInit()
        {
            VRCUiPageAuthentication authPage = GetAuthPage();

            Button doneButton = authPage.transform.Find("ButtonDone (1)").GetComponent<Button>();

            ShouldRemember = SecurePlayerPrefs.HasKey("RememberMe_Toggle");

            Button.ButtonClickedEvent doneButton_onClick = doneButton.onClick;
            doneButton.onClick = new Button.ButtonClickedEvent();
            doneButton.onClick.AddListener(new Action(() =>
            {
                if (ShouldRemember)
                {
                    if (!string.IsNullOrEmpty(authPage.loginUserName.Method_Public_String_0()) && !string.IsNullOrEmpty(authPage.loginPassword.Method_Public_String_0()))
                    {
                        SecurePlayerPrefs.SetString("RememberMe_User", authPage.loginUserName.prop_String_0, "vl9u1grTnvXA");
                        SecurePlayerPrefs.SetString("RememberMe_Pass", authPage.loginPassword.prop_String_0, "vl9u1grTnvXA");
                    }
                }
                else
                {
                    if (SecurePlayerPrefs.HasKey("RememberMe_User"))
                        SecurePlayerPrefs.DeleteKey("RememberMe_User");
                    if (SecurePlayerPrefs.HasKey("RememberMe_Pass"))
                        SecurePlayerPrefs.DeleteKey("RememberMe_Pass");
                }
                doneButton_onClick.Invoke();
            }));

            Transform popupcheckboxtrans = GameObject.Find("UserInterface/MenuContent/Popups/PerformanceSettingsPopup").GetComponent<PopupPerformanceOptions>().dynamicBoneOptionsPage.transform.Find("Checkboxes/LimitDynamicBoneUsage");

            GameObject newcheckbox = GameObject.Instantiate(popupcheckboxtrans.gameObject, authPage.loginUserName.transform.parent);
            newcheckbox.name = "RememberMe";
            GameObject.Destroy(newcheckbox.transform.GetChild(2).gameObject);
            GameObject.Destroy(newcheckbox.transform.GetChild(3).gameObject);
            GameObject.Destroy(newcheckbox.transform.GetChild(4).gameObject);
            GameObject.Destroy(newcheckbox.transform.GetChild(5).gameObject);

            Transform newcheckbox_desc_trans = newcheckbox.transform.Find("Description");
            newcheckbox_desc_trans.GetComponent<Text>().text = "Remember Me";

            Toggle newcheckbox_toggle = newcheckbox.GetComponent<Toggle>();
            newcheckbox_toggle.isOn = ShouldRemember;
            newcheckbox_toggle.onValueChanged = new Toggle.ToggleEvent();
            newcheckbox_toggle.onValueChanged.AddListener(new Action<bool>((newvalue) =>
            {
                ShouldRemember = newvalue;
                if (!ShouldRemember)
                    SecurePlayerPrefs.DeleteKey("RememberMe_Toggle");
                else
                    SecurePlayerPrefs.SetString("RememberMe_Toggle", "true", "vl9u1grTnvXA");
            }));

            newcheckbox.transform.localPosition = new Vector3((authPage.loginPassword.transform.localPosition.x - 130), (authPage.loginPassword.transform.localPosition.y - 45), authPage.loginPassword.transform.localPosition.z);
            authPage.loginUserName.transform.localPosition = new Vector3(authPage.loginUserName.transform.localPosition.x, (authPage.loginUserName.transform.localPosition.y + 30), authPage.loginUserName.transform.localPosition.z);
            authPage.loginPassword.transform.localPosition = new Vector3(authPage.loginPassword.transform.localPosition.x, (authPage.loginPassword.transform.localPosition.y + 30), authPage.loginPassword.transform.localPosition.z);

            ValidateTextTargetMethod = typeof(InputFieldValidator).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Single(it => it.GetParameters().Length == 1 && XrefScanner.XrefScan(it).Any(jt => jt.Type == XrefType.Global && jt.ReadAsObject()?.ToString() == "^([\\w\\.\\-\\+]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$"));

            InputFieldCheck();
            VRCUiManager.prop_VRCUiManager_0.field_Private_Action_1_VRCUiPage_0 = (
                (VRCUiManager.prop_VRCUiManager_0.field_Private_Action_1_VRCUiPage_0 == null)
                ? new Action<VRCUiPage>(OnPageShown)
                : Il2CppSystem.Delegate.Combine(VRCUiManager.prop_VRCUiManager_0.field_Private_Action_1_VRCUiPage_0, (UnityEngine.Events.UnityAction<VRCUiPage>)new Action<VRCUiPage>(OnPageShown)).Cast<Il2CppSystem.Action<VRCUiPage>>()
                );
        }

        private static VRCUiPageAuthentication GetAuthPage()
        {
            if (PageAuthentication == null)
                PageAuthentication = Resources.FindObjectsOfTypeAll<VRCUiPageAuthentication>().First((p) => (p.gameObject.name == "LoginUserPass"));
            return PageAuthentication;
        }

        private static void OnPageShown(VRCUiPage page)
        {
            VRCUiPageAuthentication authPage = GetAuthPage();
            if ((page != null) && (authPage != null) && (page == authPage))
                InputFieldCheck();
        }

        private static void InputFieldCheck()
        {
            VRCUiPageAuthentication authPage = GetAuthPage();
            if (authPage != null)
            {
                if (ShouldRemember)
                {
                    if (SecurePlayerPrefs.HasKey("RememberMe_User") && SecurePlayerPrefs.HasKey("RememberMe_Pass"))
                    {
                        authPage.loginUserName.field_Private_String_0 = SecurePlayerPrefs.GetString("RememberMe_User", "vl9u1grTnvXA");
                        authPage.loginUserName.prop_String_0 = authPage.loginUserName.field_Private_String_0;
                        ValidateTextTargetMethod.Invoke(authPage.loginUserName.GetComponent<InputFieldValidator>(), new object[] { authPage.loginUserName.field_Private_String_0 });

                        authPage.loginPassword.field_Private_String_0 = SecurePlayerPrefs.GetString("RememberMe_Pass", "vl9u1grTnvXA");
                        authPage.loginPassword.prop_String_0 = authPage.loginPassword.field_Private_String_0;
                        ValidateTextTargetMethod.Invoke(authPage.loginPassword.GetComponent<InputFieldValidator>(), new object[] { authPage.loginPassword.field_Private_String_0 });
                    }
                }
                else
                {
                    if (SecurePlayerPrefs.HasKey("RememberMe_User"))
                        SecurePlayerPrefs.DeleteKey("RememberMe_User");
                    if (SecurePlayerPrefs.HasKey("RememberMe_Pass"))
                        SecurePlayerPrefs.DeleteKey("RememberMe_Pass");
                }
            }
        }
    }
}