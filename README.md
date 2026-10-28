# Subnautica VR Avatar Contact Feedback

A work-in-progress BepInEx mod for **Subnautica VR** that aims to provide short controller-haptic pulses when the player makes contact with environmental surfaces.

The project is being developed as part of a graduation project exploring whether physical feedback can support spatial awareness and exploration in virtual reality.

> **Project status:** Early development. The initial release is not available yet.

## Planned behaviour

- Detect contact between the VR avatar or tracked hands and environmental geometry.
- Send a brief haptic pulse to the corresponding VR controller.
- Avoid continuous vibration while contact is maintained by using a cooldown or contact-entry rule.
- Keep the feature easy to enable or disable for research comparisons.

## Requirements

The final mod is expected to require:

- Subnautica (PC version)
- BepInEx 5
- SubmersedVR
- A SteamVR/OpenVR-compatible VR setup with motion controllers

Exact supported game, BepInEx, and SubmersedVR versions will be documented once the first working build is tested.

## Installation

Installation instructions will be added with the first release. Until then, this repository is primarily source code and development documentation.

## Building from source

1. Install Visual Studio 2022 (or another IDE that supports .NET Framework projects) with the .NET desktop development workload.
2. Clone this repository.
3. Open `VR Avatar Contact Feedback Mod.slnx`.
4. Restore the NuGet packages and build the project in Release configuration.
5. Copy the compiled mod DLL to the game's `BepInEx/plugins` folder alongside its required dependencies.

Do not commit or redistribute Subnautica game files, game assemblies, or third-party DLLs unless their licenses explicitly allow it.

## Development notes

The first technical milestone is an isolated left/right controller vibration test. Once that works, the project will validate how SubmersedVR represents hand or avatar contact before connecting contact events to haptics.

## Credits

- [BepInEx](https://github.com/BepInEx/BepInEx) for the mod-loading framework.
- [Nautilus](https://github.com/SubnauticaModding/Nautilus) for Subnautica modding support.
- [SubmersedVR](https://github.com/Okabintaro/SubmersedVR) for motion-controller VR support.

Subnautica is developed and published by Unknown Worlds Entertainment. This is an unofficial fan-made mod and is not affiliated with or endorsed by Unknown Worlds Entertainment.

## License

This project is intended to be released under the [MIT License](LICENSE). Add the `LICENSE` file before publishing if you select a different license.
