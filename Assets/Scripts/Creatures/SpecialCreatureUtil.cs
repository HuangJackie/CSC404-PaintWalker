using UnityEngine;
using static GameConstants;

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
            Paints activationPaintType1,
            Paints activationPaintType2,
            int paintQuantity1,
            int paintQuantity2,
            Material material,
            Color color)
        {
            bool shouldActivateSpecialCreature = !isPainted
                                                 && isMouseOver
                                                 && Vector3.Distance(playerPosition,
                                                     creaturePosition) < 3
                                                 && manager.GetPaintQuantity(activationPaintType1) >= paintQuantity1
                                                 && manager.GetPaintQuantity(activationPaintType2) >= paintQuantity2;
            
            if (shouldActivateSpecialCreature)
            {
                manager.DecreasePaint(activationPaintType1, paintQuantity1);
                manager.DecreasePaint(activationPaintType2, paintQuantity2);
                material.color = color;
                Debug.Log("Activate");

                return true;
            }

            return false;
        }
    }
}