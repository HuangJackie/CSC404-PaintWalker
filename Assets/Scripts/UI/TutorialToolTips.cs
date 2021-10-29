using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class TutorialToolTips : Interactable, TooltipObject
{
    // Start is called before the first frame update
    public static bool ToolTipOpened = false;
    public GameObject player;
    private LevelManager _levelManager;

    public GameObject ToolTipWalking;
    public GameObject ToolTipDoor;
    public GameObject ToolTipYellow;
    public GameObject ToolTipRed;
    public GameObject ToolTipGreen;
    public GameObject ToolTipBlue;
    private String HoverText;
    
    private UpdateUI _updateUI;

    private GameObject ToolTipUI;
    // Update is called once per frame
    private void Start()
    {
        _updateUI = FindObjectOfType<UpdateUI>();
        
        player = GameObject.FindWithTag("Player");
        switch (name)
        {
            case "Yellow":
                ToolTipUI = ToolTipYellow;
                HoverText = "About Yellow paint";
                break;
            case "Red":
                ToolTipUI = ToolTipRed;
                HoverText = "About Red paint";
                break;
            case "Green":
                ToolTipUI = ToolTipGreen;
                HoverText = "About Green paint";
                break;
            case "Blue":
                ToolTipUI = ToolTipBlue;
                HoverText = "About Blue paint";
                break;
            case "Door":
                ToolTipUI = ToolTipDoor;
                HoverText = "About exit doors";
                break;
            case "Walking":
                ToolTipUI = ToolTipWalking;
                HoverText = "About Walking, press f";
                break;
        }
        // ToolTipUI = ToolTipYellow;
    }

    void Update()
    {
        
        if (Vector3.Distance(player.transform.position, transform.position) < 3)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (ToolTipOpened)
                {
                    CloseToolTip();
                }
                else
                {
                    OpenToolTip();
                }
            }
        }
        
    }

    void CloseToolTip()
    {
        ToolTipUI.SetActive(false);
        ToolTipOpened = false;
        _levelManager.freeze_player = false;
    }

    void OpenToolTip()
    {
     ToolTipUI.SetActive(true);
     ToolTipOpened = true;
     _levelManager.freeze_player = true;
    }
    
    
    new void OnMouseOver()
    {
        base.OnMouseOver();
        IsMouseOver = true;
        OnDisplayTooltip();
    }
    private new void OnMouseExit()
    {
        base.OnMouseExit();
        if (this)
        {
            OnExitTooltip();
        }
    }

    public void OnDisplayTooltip()
    {
        
        _updateUI.SetInfoText(HoverText);
        HighlightForHoverover();
    }

    public void OnExitTooltip()
    {

        _updateUI.WipeInfoText();
        UndoHighlight();
    }
}
