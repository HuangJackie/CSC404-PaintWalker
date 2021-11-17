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
}
