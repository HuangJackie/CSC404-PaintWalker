using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using UnityEngine.Audio;

// NOTE: This class need constant maintenance and
// addition/removal of level-loading methods as new
// ones are added and old ones are removed.
public class LevelSelectMenu : SecondaryMenu
{
    private Levels levelToLoad;
    public AudioSource menuMusic;
    public AudioMixerSnapshot menuMusicOut;
   
    public void LoadTutorialColors()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.TutorialColors;
        menuMusicOut.TransitionTo(2f);

    }

    public void LoadAlpha()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.AlphaScenev2;
        menuMusicOut.TransitionTo(2f);

    }
    
    public void LoadTut1()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Tutorial1;
        menuMusicOut.TransitionTo(2f);

    }

    public void LoadTut15()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Tutorial15;
        menuMusicOut.TransitionTo(2f);

    }
    
    public void LoadTut2()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Tutorial2;
        menuMusicOut.TransitionTo(2f);

    }

    public void LoadLevel1()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Level1;
        menuMusicOut.TransitionTo(2f);

    }
    
    public void LoadLevel2()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Level2;
        menuMusicOut.TransitionTo(2f);

    }
    
    public void LoadLevel3()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
        levelToLoad = Levels.Level3;
        menuMusicOut.TransitionTo(2f);

    }

    public void OnFadeComplete()
    {
        SceneLoader.LoadLevel(levelToLoad);
    }
}
