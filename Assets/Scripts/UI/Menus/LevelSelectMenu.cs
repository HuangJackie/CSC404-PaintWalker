using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

// NOTE: This class need constant maintenance and
// addition/removal of level-loading methods as new
// ones are added and old ones are removed.
public class LevelSelectMenu : SecondaryMenu
{
    private Levels levelToLoad;
   
    public void LoadTutorialColors()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.TutorialColors;
    }

    public void LoadAlpha()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.AlphaScenev2;
    }
    
    public void LoadTut1()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Tutorial1;
    }

    public void LoadTut15()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Tutorial15;
    }
    
    public void LoadTut2()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Tutorial2;
    }

    public void LoadLevel1()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Level1;
    }

    public void OnFadeComplete()
    {
        SceneLoader.LoadLevel(levelToLoad);
    }
}
