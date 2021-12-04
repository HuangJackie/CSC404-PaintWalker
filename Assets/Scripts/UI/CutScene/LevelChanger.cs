using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;
    private int levelToLoad;
    // Update is called once per frame
    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void LoadTutorialColors(Levels levelname)
    {
        this.animator.SetTrigger("FadeOut");

    }


}
