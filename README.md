# Subnautica VR Avatar Contact Feedback

A work-in-progress BepInEx mod for **Subnautica VR** that aims to provide short controller-haptic pulses when the player makes contact with environmental surfaces.

The project has been developed as part of a graduation project exploring whether physical feedback can support spatial awareness and exploration in virtual reality.

> **Project status:** Development finished. The initial release has been uploaded.

## Behaviour

- Detects contact between the VR avatar or tracked hands and environmental geometry.
- Sends a brief haptic pulse to the corresponding VR controller upon contact.
- Avoids continuous vibration while contact is maintained in a single position by using a isTouching state.
- If the avatar remains in contact with an object surface and makes scrubbing movements instead of release, sends a continuous slight vibration as a feedback of scrubbing.

## Requirements

This mod requires:

- Subnautica (PC version)
- BepInEx 5
- SubmersedVR
- A SteamVR/OpenVR-compatible VR setup with motion controllers

## Installation

1. Download the [zip file](https://github.com/Emiryuu/Subnautica-VR-Contact-Feedback/releases/tag/v1.0) in Release.
1. Unzip and place the `VR_Avatar_Contact_Feedback_Mod.dll` file inside the plugins folder of BepInEx.

## Building from source

1. Install Visual Studio 2022 (or another IDE that supports .NET Framework projects) with the .NET desktop development workload.
2. Clone this repository.
3. Open `VR Avatar Contact Feedback Mod.slnx`.
4. Restore the NuGet packages and build the project in Release configuration.
5. Copy the compiled mod DLL to the game's `BepInEx/plugins` folder alongside its required dependencies.

Do not commit or redistribute Subnautica game files, game assemblies, or third-party DLLs unless their licenses explicitly allow it.

## Credits

- [BepInEx](https://github.com/BepInEx/BepInEx) for the mod-loading framework.
- [Nautilus](https://github.com/SubnauticaModding/Nautilus) for Subnautica modding support.
- [SubmersedVR](https://github.com/Okabintaro/SubmersedVR) for motion-controller VR support.

Subnautica is developed and published by Unknown Worlds Entertainment. This is an unofficial fan-made mod and is not affiliated with or endorsed by Unknown Worlds Entertainment.

## License

This project is intended to be released under the [MIT License](LICENSE). Add the `LICENSE` file before publishing if you select a different license.
