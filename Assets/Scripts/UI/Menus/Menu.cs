using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Menu : MonoBehaviour
{
    protected ControllerUtil controllerUtil;
    protected List<MenuButton> menuButtons;
    protected int highlightedButton;

    protected virtual void Start()
    {
        controllerUtil = FindObjectOfType<ControllerUtil>();
        PopulateButtons(transform);
    }

    protected virtual void Update()
    {
        if (controllerUtil.GetConfirmButtonPressed())
        {
            print("Selecting highlightedButton: " + highlightedButton);
            menuButtons[highlightedButton].Trigger();
        }
            
        if (controllerUtil.GetGameMenuSelectAxis(out int selectDirection))
        {
            highlightedButton += selectDirection;
            if (highlightedButton < 0) highlightedButton = menuButtons.Count - 1;
            if (highlightedButton > menuButtons.Count - 1) highlightedButton = 0;
            ChangeHighlightedButton(highlightedButton);
        }
    }

    protected void PopulateButtons(Transform callingMenu)
    {
        bool foundButtonToSelect = false;
        highlightedButton = -1;
        menuButtons = new List<MenuButton>();

        int currMenuButtonIndex = 0;
        foreach (Transform child in callingMenu)
        {
            // If the current child has a MenuButton component
            MenuButton currMenuButton = child.GetComponent<MenuButton>();
            if (currMenuButton != null)
            {
                // Select the first child with a MenuButton component
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

        if (highlightedButton > -1)
        {
            ChangeHighlightedButton(highlightedButton);
        }
    }

    public void ChangeHighlightedButton(int buttonIndex)
    {
        highlightedButton = Mathf.Clamp(buttonIndex, 0, menuButtons.Count - 1);
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
