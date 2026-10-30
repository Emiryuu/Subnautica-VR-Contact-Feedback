using BepInEx;
using BepInEx.Logging;
using System;
using System.Reflection;
using UnityEngine;

namespace VR_Avatar_Contact_Feedback_Mod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("VRACF loaded. Press F9 in a loaded save to test the right-controller haptic pulse.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                PlayRightTestPulse();
            }
        }

        private void PlayRightTestPulse()
        {
            Assembly actionsAssembly = FindAssembly("SteamVR_Actions");
            Type actionsType = actionsAssembly?.GetType("Valve.VR.SteamVR_Actions");

            if (actionsType == null)
            {
                Logger.LogWarning("SteamVR haptic actions are not ready yet.");
                return;
            }

            object hapticAction =
                actionsType.GetField("subnautica_HapticsRight", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                actionsType.GetProperty("subnautica_HapticsRight", BindingFlags.Public | BindingFlags.Static)?.GetValue(null, null);

            if (hapticAction == null)
            {
                Logger.LogWarning("The right haptic action was not found.");
                return;
            }

            MethodInfo executeMethod = null;

            foreach (MethodInfo method in hapticAction.GetType().GetMethods())
            {
                if (method.Name == "Execute" && method.GetParameters().Length == 5)
                {
                    executeMethod = method;
                    break;
                }
            }

            if (executeMethod == null)
            {
                Logger.LogWarning("The SteamVR haptic Execute method was not found.");
                return;
            }

            Type inputSourceType = executeMethod.GetParameters()[4].ParameterType;
            object anyInputSource = Enum.Parse(inputSourceType, "Any");

            executeMethod.Invoke(
                hapticAction,
                new object[] { 0f, 0.12f, 10f, 0.35f, anyInputSource });

            Logger.LogInfo("Sent VRACF right-controller test pulse.");
        }

        private static Assembly FindAssembly(string assemblyName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return assembly;
                }
            }

            return null;
        }
    }
}