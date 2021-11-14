using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorage : MonoBehaviour
{
    public static List<GameObject> blockStorage;
    public static List<GameObject> paintOrbStorage;
    public static List<SpecialCreature> specialCreatureStorage;
    public static List<GameObject> wallStorage;
    public static List<List<dynamic>> blockStates;
    public static List<List<dynamic>> paintOrbStates;
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

    public void AddBlock(GameObject block)
    {
        blockStorage.Add(block);
    }
}
