This demo needs to be started from the main menu scene (MainMenu_StartHere.unity). If you start it from the levels you might encounter unpredictable behaviour.

Additionally when you start a vertical level with a forced aspect ratio, the black screen on the sides might not be visible. But this is only in the editor and in the build, the sides will be covered with black.

You will also need to bake the lighting before playing or building the scenes in the main menu folder, just go to window, rendering, lighting and uncheck auto generate, then click on generate lighting and save the scene for every scene you intend to build. 
For more on lighting you can see the Unity manual.
We didn't bake the lighting beforehand because Unity versions might have different lighting settings and give you warning or error message that you need to rebake the lighting.

Finally just make sure your build settings are correct or have the same index numbers which are in the game manager.