using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* MenuToggle should be attached to all Menu buttons with
 * a toggle component. Requires a Menu parent
 */
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button self;
    private Menu parentMenu;
    [SerializeField] private Image selectionIndication;

    // The index in which it appears in its parentMenu
    // Must be set by parentMenu upon initializing its MenuToggle children
    public int index = -1;

    private void Start()
    {
        self = GetComponent<Button>();

        // Get parenting Menu. If no Menu in parent, the parent must
        // be an empty renderer GameObject, so its parent must be a Menu
        parentMenu = GetComponentInParent<Menu>();
        if (parentMenu == null) transform.parent.GetComponentInParent<Menu>();
    }

    // Triggers the onClick methods defined in the inspector
    public void Trigger()
    {
        self.onClick.Invoke();
    }

    public void DisplayAsSelected(bool selected)
    {
        Color selectionIndicationColor = selectionIndication.color;
        selectionIndicationColor.a = selected ? 1 : 0;
        selectionIndication.color = selectionIndicationColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Will automatically call DisplayAsSelected()
        parentMenu.ChangeHighlightedButton(index);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Trigger();
    }
}
