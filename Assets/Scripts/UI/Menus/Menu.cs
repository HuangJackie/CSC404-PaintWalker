using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private ControllerUtil controllerUtil;
    private List<MenuButton> menuButtons;
    private int highlightedButton;

    protected virtual void Start()
    {
        controllerUtil = FindObjectOfType<ControllerUtil>();
        PopulateButtons();
    }

    void Update()
    {
        if (controllerUtil.GetConfirmButtonPressed())
        {
            controllerUtil.CloseMenu();
            menuButtons[highlightedButton].Trigger();
        }
            
        if (controllerUtil.GetGameMenuSelectAxis(out int selectDirection))
        {
            highlightedButton += selectDirection;
            if (highlightedButton < 0) highlightedButton = menuButtons.Count;
            if (highlightedButton > menuButtons.Count - 1) highlightedButton = 0;
            ChangeHighlightedButton(highlightedButton);
        }
    }

    private void PopulateButtons()
    {
        bool foundButtonToSelect = false;
        highlightedButton = -1;
        menuButtons = new List<MenuButton>();

        int currMenuButtonIndex = 0;
        foreach (Transform child in transform)
        {
            // If the current child has a MenuButton component
            MenuButton currMenuButton = child.GetComponent<MenuButton>();
            if (currMenuButton != null)
            {
                // Select the first child with a MenuToggle component
                if (!foundButtonToSelect)
                {
                    highlightedButton = currMenuButtonIndex;
                    foundButtonToSelect = true;
                }

                menuButtons.Add(currMenuButton);
                currMenuButton.index = currMenuButtonIndex;
                currMenuButtonIndex++;
            }
        }
    }

    public void ChangeHighlightedButton(int buttonIndex)
    {
        highlightedButton = Mathf.Clamp(buttonIndex, 0, menuButtons.Count);
        menuButtons[highlightedButton].DisplayAsSelected(true);

        // Disable the selectionIndication Image of all other buttons
        foreach (MenuButton button in menuButtons)
        {
            if (button.index != highlightedButton)
            {
                menuButtons[button.index].DisplayAsSelected(false);
            }
        }
    }
}
