using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

// NOTE: This class need constant maintenance and
// addition/removal of level-loading methods as new
// ones are added and old ones are removed.
public class LevelSelectMenu : SecondaryMenu
{
    public static void LoadTutorialColors()
    {
        SceneLoader.LoadLevel(Levels.TutorialColors);
    }

    public static void LoadAlpha()
    {
        SceneLoader.LoadLevel(Levels.AlphaScenev2);
    }
    
    public static void LoadTut1()
    {
        SceneLoader.LoadLevel(Levels.Tutorial1);
    }

    public static void LoadTut15()
    {
        SceneLoader.LoadLevel(Levels.Tutorial15);
    }
    
    public static void LoadTut2()
    {
        SceneLoader.LoadLevel(Levels.Tutorial2);
    }

    public static void LoadLevel1()
    {
        SceneLoader.LoadLevel(Levels.Level1);
    }
}
