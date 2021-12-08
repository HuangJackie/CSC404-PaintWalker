using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorage : MonoBehaviour
{
    public static List<GameObject> blockStorage;
    public static List<GameObject> paintOrbStorage;
    public static List<SpecialCreature> specialCreatureStorage;
    public static List<GameObject> wallStorage;
    public static List<GameObject> footStepStorage;
    public static List<GameObject> sparkleStorage;
    public static List<List<dynamic>> blockStates;
    public static List<List<dynamic>> paintOrbStates;
    public static List<List<dynamic>> footPrintStates;
    public static List<List<dynamic>> sparkleStates;
    public static List<List<dynamic>> specialCreatureStates;
    public static List<List<dynamic>> wallStates;
    public static List<dynamic> paintStates;
    void Awake()
    {
        blockStorage = new List<GameObject>();
        paintOrbStorage = new List<GameObject>();
        specialCreatureStorage = new List<SpecialCreature>();
        wallStorage = new List<GameObject>();
        paintStates = new List<dynamic>();
        footStepStorage = new List<GameObject>();
        sparkleStorage = new List<GameObject>();

    }

    // Update is called once per frame
    public static void UpdateBlockStorage()
    {
        blockStates = new List<List<dynamic>>();
        foreach (GameObject block in blockStorage)
        {
            Ground groundScript = block.GetComponent<Ground>();
            List<dynamic> blockInfo = new List<dynamic>();
            blockInfo.Add(block.transform.position);
            blockInfo.Add(groundScript.isPaintedByBrush);
            blockInfo.Add(groundScript.isPaintedByFeet);
            blockInfo.Add(groundScript.originalColour);
            blockInfo.Add(groundScript.paintedColour);
            blockInfo.Add(block.GetComponentInChildren<Renderer>().material.color);
            blockInfo.Add(block.activeSelf);
            blockInfo.Add(groundScript._cur_model);
            blockInfo.Add(groundScript.destinationNeutral);
            blockInfo.Add(groundScript._paintedColour);
            blockInfo.Add(groundScript.isWalkedOverHorizontally);
            blockInfo.Add(groundScript.isWalkedOverVertially);
            blockInfo.Add(block.layer);
            blockInfo.Add(groundScript._isIceBlockEffectEnabled);
            blockInfo.Add(groundScript.stillMoving);
            blockStates.Add(blockInfo);
        }
    }

    public static void UpdatePaintOrbStorage()
    {
        paintOrbStates = new List<List<dynamic>>();
        foreach (GameObject paintOrb in paintOrbStorage)
        {
            List<dynamic> paintOrbInfo = new List<dynamic>();
            paintOrbInfo.Add(paintOrb.transform.position);
            paintOrbInfo.Add(paintOrb.activeSelf);
            paintOrbStates.Add(paintOrbInfo);
        }
    }

    public static void UpdateSpecialCreatureStorage()
    {
        specialCreatureStates = new List<List<dynamic>>();
        foreach (SpecialCreature specialCreature in specialCreatureStorage)
        {
            List<dynamic> specialCreatureInfo = new List<dynamic>();
            specialCreatureInfo.Add(specialCreature.gameObject.transform.position);
            //if (specialCreature is HowlingCreature)
            //{
            //    specialCreatureInfo.Add(((HowlingCreature)specialCreature).isTriggered);
            //}
            specialCreatureInfo.Add(specialCreature.isPainted);
            specialCreatureInfo.Add(specialCreature.originalColour);
            specialCreatureInfo.Add(specialCreature.paintedColour);
            specialCreatureInfo.Add(specialCreature.gameObject.activeSelf);
            specialCreatureInfo.Add(specialCreature.frozen_model.activeSelf);
            specialCreatureInfo.Add(specialCreature.coloured_model.activeSelf);
            specialCreatureStates.Add(specialCreatureInfo);
        }
    }

    public static void UpdateWallStorage()
    {
        wallStates = new List<List<dynamic>>();
        foreach (GameObject wall in wallStorage)
        {
            List<dynamic> wallInfo = new List<dynamic>();
            wallInfo.Add(wall.gameObject.transform.position);
            wallInfo.Add(wall.GetComponent<MoveWall>().operate);
            wallStates.Add(wallInfo);
        }
    }

    public static void UpdateFootPrintStorage()
    {
        footPrintStates = new List<List<dynamic>>();
        foreach (GameObject footStep in footStepStorage)
        {
            List<dynamic> footStepInfo = new List<dynamic>();
            
            // footStep.gameObject can be null after loading an existing checkpoint and hitting another one.
            if (footStep.gameObject != null)
            {
                footStepInfo.Add(footStep.gameObject.activeSelf);
                footPrintStates.Add(footStepInfo);
            }
        }
        print("update done");
    }

    public static void UpdateSparkleStorage()
    {
        sparkleStates = new List<List<dynamic>>();
        foreach (GameObject sparkle in sparkleStorage)
        {
            List<dynamic> sparkleInfo = new List<dynamic>();
            // sparkle.gameObject can be null after loading an existing checkpoint and hitting another one.
            if (sparkle.gameObject != null)
            {
                sparkleInfo.Add(sparkle.gameObject.activeSelf);
                sparkleStates.Add(sparkleInfo);
            }
        }
        print(sparkleStates.Count);
        print("sparkle update done");
    }

    public void AddBlock(GameObject block)
    {
        blockStorage.Add(block);
    }

    public void AddFootPrint(GameObject footPrint)
    {
        footStepStorage.Add(footPrint);
    }

    public void AddSparkle(GameObject sparkle)
    {
        sparkleStorage.Add(sparkle);
    }

    public static void WipeStorage()
    {
        paintStates = new List<dynamic>();
    }
}
