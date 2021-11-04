using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private ControllerUtil _controllerUtil;
    public GameObject start;
    public GameObject tutorial;
    private Button[] _menuOptions;

    private int _selectedMenuOption;
    private const int TotalNumberOfMenuOptions = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        _controllerUtil = FindObjectOfType<ControllerUtil>();

        _menuOptions = new Button[TotalNumberOfMenuOptions];
        _menuOptions[0] = start.GetComponentInChildren<Button>();
        _menuOptions[1] = tutorial.GetComponentInChildren<Button>();
        _selectedMenuOption = 0;
        _menuOptions[_selectedMenuOption].OnPointerEnter(null);
    }

    // Update is called once per frame
    void Update()
    {
        if (_controllerUtil.GetConfirmButtonPressed())
        {
            _menuOptions[_selectedMenuOption].onClick.Invoke();
            _selectedMenuOption = 0;
            if (_selectedMenuOption == 0)
            {
                _controllerUtil.CloseMenu();
            }
        }
            
        if (_controllerUtil.GetGameMenuSelectAxis(out int select))
        {

            _menuOptions[_selectedMenuOption].OnPointerExit(null);
            if (select > 0)
            {
                IncrementMenuOption();
            }
            else
            {
                DecrementMenuOption();
            }

            _menuOptions[_selectedMenuOption].OnPointerEnter(null);
        }
    }
    
    private void DecrementMenuOption()
    {
        _selectedMenuOption--;
        if (_selectedMenuOption == -1)
        {
            _selectedMenuOption = TotalNumberOfMenuOptions - 1;
        }
    }

    private void IncrementMenuOption()
    {
        _selectedMenuOption++;
        if (_selectedMenuOption == TotalNumberOfMenuOptions)
        {
            _selectedMenuOption = 0;
        }
    }
}
