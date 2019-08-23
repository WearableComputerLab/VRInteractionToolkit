# 3D User Interaction Toolkit
![alt text](https://i.imgur.com/xVQx4pW.jpg) <p align="center">This image presents ten of the twenty-five interaction techniques implemented in 3DUITK</p>
### This toolkit is designed for immersive 3D User Interaction and contains implementations of twenty-five selection and manipulation techniques proposed from thirty years of prior research.

# Documentation
Full documentation and additional information about interaction techniques are provided on our GitHub Wiki: https://github.com/WearableComputerLab/VRInteractionToolkit/wiki

A six page research poster-paper on the 3D User Interaction Toolkit which was published and accepted at ISMAR19 conference can be found here:

Our full systems requirement document can be downloaded here:

## Getting Started
### Setting Up
1. The repository can be downloaded or cloned directly from the GitHub repository OR (coming soon) can be downloaded directly from the Unity Asset Store.
2. Import the project into an existing Unity project OR Open the downloaded project in Unity.
3. Each interaction technique is seperated into individual folders which is named based on the official name of the interaction technique. Each folder contains the prefabs, scripts, materials, and sprites associated with the given interaction technique. The Technique Example Scenes folder contains an example scene for each interaction technique, the Technique Prefabs folder provides a reference to each interaction technique. The interaction technique can be implemented by dragging the prefab from the Unity directory into the scene.
4. If using SteamVR ensure the correct SteamVR camerarig has been added into the Unity scene.
### Dependencies
3DUITK supports SteamVR Legacy, SteamVR 2, and alternative platform. This can be managed through the Dependencies Manager script which is attached to each prefab implementation. Within the inspector developers can specify the following SteamVR Versions:
1. SteamVR_Legacy - which uses the SteamVR 1 input system. This is preferred as 3DUITK was originally built and heavily tested off this version.
2. SteamVR_2 - Uses the SteamVR 2 - 2.2 input system. (This requires developers to set up their input bindings and attach within the prefabs inspector parameters.
#### For developers who wish to set 3DUITK up on alternative platforms (e.g. HoloLens, Android, Google Cardboard etc..)
3. None - This option completely removes the SteamVR dependencies to allow for alternative platforms to be integrated with 3DUITK. A reference to the controller or tracked gameobject can be dragged into the prefabs 'TrackedObject' inspector parameter. (e.g. if using Android, the Unity gameobject referencing the phone camera is dragged into the prefabs 'TrackedObj' inspector parameter.)


# Disclaimer

## Reference
All techniques implemented in this project are based off the research of other people, and have been acquired from the textbook '3D User Interfaces theory and practice'
Full reference below:
LaViola, J., Kruijff, E., McMahan, R., Bowman, D. and Poupyrev, I. (2017). 3D user interfaces. 2nd ed. Boston: Addison-Wesley, pp.256-315.

## License
Code is released under the MIT License.
