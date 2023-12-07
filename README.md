# AR_Project_Visualization
 
1. In your Unity project, drag **MarkerManager.cs** and **IRToolController.cs** in **Assets\HoloLensIRTracking\Scripts\IRToolTrack** into your GameObject running the tracking codes as components.
2. You should drag **IRToolController.cs** into the ir tool controller slot in **Marker Manager** component.
3. Drag prefabs **tool1_test** and **reference1_test** into **marker prefab** and **reference prefab** slots in the Marker Manager component.
4. Type the paths of every tool configuration file in Tool_Config_Path slot, for example: "Assets/MarkerConfigs/tool1.json, /path2.json". Type the paths of the reference configuration file in Reference_Config_Path slot, for example: "Assets/MarkerConfigs/reference1.json". For now, we define reference as Id=0, Ids of tools begin from 1.
5. You can update the reference pose once by pressing 'b' button in game mode.
