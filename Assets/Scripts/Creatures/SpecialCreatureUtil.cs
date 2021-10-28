using UnityEngine;

namespace DefaultNamespace
{
    public static class SpecialCreatureUtil
    {
        public static bool ActivateSpecialCreature(
            bool isPainted,
            bool isMouseOver,
            Vector3 playerPosition,
            Vector3 creaturePosition,
            LevelManager manager,
            string paintColour1,
            string paintColour2,
            int paintQuantity1,
            int paintQuantity2,
            Material material,
            Color color)
        {
            bool shouldActivateSpecialCreature = !isPainted
                                                 && (isMouseOver || Input.GetButtonDown("Paint"))
                                                 && Vector3.Distance(playerPosition,
                                                     creaturePosition) < 3
                                                 && manager.GetPaintQuantity(paintColour1) >= paintQuantity1
                                                 && manager.GetPaintQuantity(paintColour2) >= paintQuantity2;
            if (shouldActivateSpecialCreature)
            {
                manager.DecreasePaint(paintColour1, paintQuantity1);
                manager.DecreasePaint(paintColour2, paintQuantity2);
                material.color = color;
                Debug.Log("Activate");

                return true;
            }

            return false;
        }
    }
}