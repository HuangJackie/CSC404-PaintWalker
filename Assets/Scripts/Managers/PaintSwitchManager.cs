using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PaintSwitchManager : MonoBehaviour
{
    private LevelManager _levelManager;
    private ControllerUtil _controllerUtil;

    // Start is called before the first frame update
    void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    // Update is called once per frame
    void Update()
    {
        ListenForSwitchColours();
    }

    private void ListenForSwitchColours()
    {
        if (_controllerUtil.GetSwitchGreenPressed())
        {
            _levelManager.ChangePaint(GameConstants.GreenColour);
        }
        
        if (_controllerUtil.GetSwitchYellowPressed())
        {
            _levelManager.ChangePaint(GameConstants.YellowColour);
        }
        
        if (_controllerUtil.GetSwitchRedPressed())
        {
            _levelManager.ChangePaint(GameConstants.RedColour);
        }
        
        if (_controllerUtil.GetSwitchBluePressed())
        {
            _levelManager.ChangePaint(GameConstants.BlueColour);
        }
    }
}
