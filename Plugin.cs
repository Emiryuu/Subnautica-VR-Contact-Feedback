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
        private bool leftWasTouching;
        private bool rightWasTouching;
        private Vector3 leftPreviousPosition;
        private Vector3 rightPreviousPosition;
        private bool hasLeftPreviousPosition;
        private bool hasRightPreviousPosition;
        private float nextLeftScrubPulseTime;
        private float nextRightScrubPulseTime;

        private const float ImpactDuration = 0.08f;
        private const float ImpactFrequency = 10f;
        private const float ImpactAmplitude = 1.0f;
        private const float ScrubDuration = 0.13f;
        private const float ScrubFrequency = 15f;
        private const float ScrubAmplitude = 0.20f;
        private const float ScrubInterval = 0.14f;
        private const float MinimumScrubSpeed = 0.05f;
        private const float ContactRadius = 0.06f;
        private const float BackOfHandOffset = 0.055f;

        private void Awake()
        {
            Logger = base.Logger;
        }

        private void Update()
        {
            CheckControllerContact();
        }

        private void PlayRightPulse(float duration, float frequency, float amplitude)
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
                new object[] { 0f, duration, frequency, amplitude, anyInputSource });
        }

        private void PlayLeftPulse(float duration, float frequency, float amplitude)
        {
            Assembly actionsAssembly = FindAssembly("SteamVR_Actions");
            Type actionsType = actionsAssembly?.GetType("Valve.VR.SteamVR_Actions");

            if (actionsType == null)
            {
                Logger.LogWarning("SteamVR haptic actions are not ready yet.");
                return;
            }

            object hapticAction =
                actionsType.GetField("subnautica_HapticsLeft", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                actionsType.GetProperty("subnautica_HapticsLeft", BindingFlags.Public | BindingFlags.Static)?.GetValue(null, null);

            if (hapticAction == null)
            {
                Logger.LogWarning("The left haptic action was not found.");
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
                new object[] { 0f, duration, frequency, amplitude, anyInputSource });
        }

        private void CheckControllerContact()
        {
            Assembly submersedVRAssembly = FindAssembly("SubmersedVR");
            Type cameraRigType = submersedVRAssembly?.GetType("SubmersedVR.VRCameraRig");

            if (submersedVRAssembly == null)
            {
                Logger.LogWarning("SubmersedVR is not loaded.");
                return;
            }

            if (cameraRigType == null)
            {
                Logger.LogWarning("CameraRig type not found.");
                return;
            }

            object rigInstanceValue = cameraRigType.GetField("instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

            if (rigInstanceValue == null)
            {
                Logger.LogWarning("CameraRig instance not found.");
                return;
            }

            object leftHandTargetValue = cameraRigType.GetField("leftHandTarget")?.GetValue(rigInstanceValue);
            object rightHandTargetValue = cameraRigType.GetField("rightHandTarget")?.GetValue(rigInstanceValue);
            object leftControllerValue = cameraRigType.GetField("leftController")?.GetValue(rigInstanceValue);
            object rightControllerValue = cameraRigType.GetField("rightController")?.GetValue(rigInstanceValue);

            if (leftControllerValue == null || rightControllerValue == null || leftHandTargetValue == null || rightHandTargetValue == null)
            {
                Logger.LogWarning("Controller or hand-target objects not found.");
                return;
            }

            GameObject leftController = (GameObject)leftControllerValue;
            GameObject rightController = (GameObject)rightControllerValue;
            GameObject leftHandTarget = (GameObject)leftHandTargetValue;
            GameObject rightHandTarget = (GameObject)rightHandTargetValue;

            Vector3 leftCurrentPosition = leftController.transform.position;
            Vector3 rightCurrentPosition = rightController.transform.position;

            float leftSpeed = 0f;
            float rightSpeed = 0f;

            if (hasLeftPreviousPosition)
            {
                leftSpeed = Vector3.Distance(leftCurrentPosition, leftPreviousPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
            }
            if (hasRightPreviousPosition)
            {
                rightSpeed = Vector3.Distance(rightCurrentPosition, rightPreviousPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
            }

            leftPreviousPosition = leftCurrentPosition;
            rightPreviousPosition = rightCurrentPosition;
            hasLeftPreviousPosition = true;
            hasRightPreviousPosition = true;

            bool leftTouching = HasValidEnvironmentContact(leftCurrentPosition) ||
                                HasValidEnvironmentContact(leftHandTarget.transform.position - leftHandTarget.transform.forward * BackOfHandOffset);

            if (leftTouching && !leftWasTouching)
            {
                PlayLeftPulse(ImpactDuration, ImpactFrequency, ImpactAmplitude);
                nextLeftScrubPulseTime = Time.time + ScrubInterval;
                Logger.LogInfo("Left hand contact began.");
            } 
            else if (leftTouching && leftWasTouching && leftSpeed >= MinimumScrubSpeed && Time.time >= nextLeftScrubPulseTime)
            {
                PlayLeftPulse(ScrubDuration, ScrubFrequency, ScrubAmplitude);
                nextLeftScrubPulseTime = Time.time + ScrubInterval;
            }

            leftWasTouching = leftTouching;

            bool rightTouching = HasValidEnvironmentContact(rightCurrentPosition) ||
                                HasValidEnvironmentContact(rightHandTarget.transform.position - rightHandTarget.transform.forward * BackOfHandOffset);

            if (rightTouching && !rightWasTouching)
            {
                PlayRightPulse(ImpactDuration, ImpactFrequency, ImpactAmplitude);
                nextRightScrubPulseTime = Time.time + ScrubInterval;
                Logger.LogInfo("right hand contact began.");
            } 
            else if (rightTouching && rightWasTouching && rightSpeed >= MinimumScrubSpeed && Time.time >= nextRightScrubPulseTime)
            {
                PlayRightPulse(ScrubDuration, ScrubFrequency, ScrubAmplitude);
                nextRightScrubPulseTime = Time.time + ScrubInterval;
            }

            rightWasTouching = rightTouching;


        }

        private bool HasValidEnvironmentContact(Vector3 probePosition)
        {
            Collider[] collidedObjects = Physics.OverlapSphere(probePosition, ContactRadius);

            foreach (Collider collidedObject in collidedObjects)
            {
                if (collidedObject.gameObject.name == "Player" ||
                    collidedObject.isTrigger ||
                    (collidedObject.attachedRigidbody != null &&
                    !collidedObject.attachedRigidbody.isKinematic))
                {
                    continue;
                }
                return true;
            }
            return false;
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