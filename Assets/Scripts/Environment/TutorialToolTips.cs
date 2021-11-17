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
    public GameObject ToolTipPainting;
    public GameObject ToolTipPanning;
    public GameObject ToolTipDoor;
    public GameObject ToolTipYellow;
    public GameObject ToolTipRed;
    public GameObject ToolTipGreen;
    public GameObject ToolTipBlue;
    public GameObject ToolTipUlala;
    public GameObject ToolTipArie;
    public GameObject ToolTipGemi;
    private String HoverText;
    private bool HoverTextActive;
    
    private UpdateUI _updateUI;

    private ControllerUtil _controllerUtil;

    private GameObject ToolTipUI;
    // Update is called once per frame
    private void Start()
    {
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        Material = GetComponentInChildren<Renderer>().material;
        _updateUI = FindObjectOfType<UpdateUI>();
        _levelManager = FindObjectOfType<LevelManager>();
        HoverTextActive = false;

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
            case "Painting":
                ToolTipUI = ToolTipPainting;
                HoverText = "About Painting";
                break;
            case "Panning":
                ToolTipUI = ToolTipPanning;
                HoverText = "About Panning";
                break;
            case "Ulala":
                ToolTipUI = ToolTipUlala;
                HoverText = "About Ulala";
                break;
            case "Arie":
                ToolTipUI = ToolTipArie;
                HoverText = "About Arie";
                break;
            case "Gemi":
                ToolTipUI = ToolTipGemi;
                HoverText = "About Gemi";
                break;
            
        }
    }

    void Update()
    {
        Vector3 playerpos = player.transform.position;
        Vector3 signpos = transform.position;
        //Debug.Log("X: " + Vector3.Normalize(playerpos- signpos).x + " Z: " + Vector3.Normalize(playerpos- signpos).z);
        if ((Vector3.Distance(playerpos, signpos) < 2))
            
        // if (Vector3.Distance(player.transform.position, transform.position) < 2)
        // if (Vector3.Normalize(player.transform.position - transform.position).z < 0) 
        {
            if (!HoverTextActive)
            {
                _updateUI.SetInfoText(HoverText);
                HoverTextActive = true;
                HighlightForHoverover();
            }
            if (Input.GetKeyDown(KeyCode.F) 
                // TODO: Remove
                // || _controllerUtil.GetInteractButtonDown()
                )
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
        } else
        {
            if (HoverTextActive)
            {
                _updateUI.WipeInfoText();
                HoverTextActive = false;
                UndoHighlight();
            }
        }
        
    }

    public void CloseToolTip()
    {
        ToolTipUI.SetActive(false);
        ToolTipOpened = false;
        Time.timeScale = 1f;
        _levelManager.freeze_player = false;
    }

    public void OpenToolTip()
    {
     ToolTipUI.SetActive(true);
     ToolTipOpened = true;
     Time.timeScale = 0f;
     _levelManager.freeze_player = true;
    }
    
    
    new void OnMouseOver()
    {
        //base.OnMouseOver();
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

    public bool IsToolTipOpen()
    {
        return ToolTipOpened;
    }
}
