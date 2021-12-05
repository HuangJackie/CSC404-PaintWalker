using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void LoadTutorialColors(Levels levelname)
    {
        this.animator.SetTrigger("FadeOut");
    }
}
