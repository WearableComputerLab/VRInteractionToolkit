# 3D User Interaction Toolkit
![alt text](https://i.imgur.com/xVQx4pW.jpg) <p align="center">This image presents ten of the twenty-five interaction techniques implemented in 3DUITK</p>
### This toolkit is designed for immersive 3D User Interaction and contains implementations of twenty-five selection and manipulation techniques proposed from thirty years of prior research.

#### Below is a video demonstrating the different interaction technique implemented in 3DUITK:
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/nGMC6kkdjg8/0.jpg)](https://www.youtube.com/watch?v=nGMC6kkdjg8)


# Documentation
Full documentation and additional information about interaction techniques are provided on our GitHub Wiki: https://github.com/WearableComputerLab/VRInteractionToolkit/wiki

A six page research poster-paper on the 3D User Interaction Toolkit which was published and accepted at ISMAR19 conference can be found here: https://ieeexplore.ieee.org/abstract/document/8951937

K. May, I. Hanan, A. Cunningham and B. Thomas, "3DUITK: An Opensource Toolkit for Thirty Years of Three-Dimensional Interaction Research," 2019 IEEE International Symposium on Mixed and Augmented Reality Adjunct (ISMAR-Adjunct), 2019, pp. 175-180, doi: 10.1109/ISMAR-Adjunct.2019.00-52.

## Getting Started
### Setting Up
1. The repository can be downloaded or cloned directly from the GitHub repository.
2. Import the project into an existing Unity project OR open the downloaded project in Unity.
- ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) NOTE: Each interaction technique is seperated into individual folders named after the given interaction technique. Each folder contains the prefabs, scripts, materials, and sprites associated with the given interaction technique. The Techniques Example Scenes folder contains an example scene for each interaction technique. The Technique Prefabs folder provides a reference to each interaction technique. An interaction technique can be implemented into a scene simply by dragging the prefab from the Technique Prefabs folder into the scene.

3. Ensure the correct SteamVR CameraRig has been added into the Unity scene and Virtual Reality supported is ticked into the project player settings (Edit -> Project Settings -> Player -> XR Settings).
#### Accessing testing scenes
4. Two types of testing scenes are available with 3DUITK:
    1. A comprehensive testing environment which is available in the Technique Examples Scenes folder.
    2. A basic testing environment which can be opened within the interaction techniques default folder.
    
#### Setting up in a new scene
5. Create a new scene and delete the Main Camera.
6. Drag the [CameraRig] from inspector into the scene (From Dependencies->SteamVR).
7. Drag an Interaction Technique into the scene (From Technique_Prefabs).
  - ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) NOTE: The controllers should autonomously become attached as a reference to the interaction techniques scripts. However, if null      reference exceptions occur make sure to drag the controllers as a reference to prefabs scripts.
8. Assign the Interaction Layers. (Interaction Techniques will only interact with specific GameObjects that are specified as an interactable layer). Alternatively, to interact with all GameObjects within the scene, set the Interaction Layers to 'Everything'.
9. Set the Intearction Type to Selection, Manipulation (Movement), or Manipulation (UI) which presents a UI with additional manipulation options.

### Dependencies
3DUITK supports SteamVR Legacy, SteamVR 2.0, and alternative platforms. Setting which platform you intend to use is managed through the Dependencies Manager script which is attached to each implementation. Within the inspector dvelopers can specify the following platform options:
1. SteamVR_Legacy - Utilizes the default SteamVR 1.0 input system. This is preferred as 3DUITK was originally built and tested off this version.
2. SteamVR_2 - Utilizes the updated SteamVR 2 - 2.2 input system. This option requires developeds to set up their input bindings and attach them to the prefabs inspector parameters.
#### For developers who wish to set 3DUITK up on alternative platforms (e.g. HoloLens, Android, Google Cardboard etc..)
3. None - This option completely removes the SteamVR dependencies to allow for alternative platforms to be integrated with 3DUITK. A reference to the controller or tracked GameObject can be dragged into the prefabs 'TrackedObject' inspector parameter. (e.g. if using Android, the Unity gameobject referencing the phone camera is dragged into the prefabs 'TrackedObj' inspector parameter.).
#### Simple setup in new scene
![Alt Text](https://i.imgur.com/sASzavZ.gif)

# Disclaimer
## Authors
All software written in this project was developed by Kieran May and Ian Hanan.

## Reference
All techniques implemented in this project are based off the research of other people, and have been acquired from the textbook '3D User Interfaces theory and practice'
Full reference below:
LaViola, J., Kruijff, E., McMahan, R., Bowman, D. and Poupyrev, I. (2017). 3D user interfaces. 2nd ed. Boston: Addison-Wesley, pp.256-315.

## License
Code is released under the MIT License.
