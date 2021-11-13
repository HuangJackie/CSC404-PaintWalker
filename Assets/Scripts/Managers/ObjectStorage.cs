using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorage : MonoBehaviour
{
    public static List<GameObject> blockStorage;
    public static List<List<dynamic>> blockStates;
    void Awake()
    {
        blockStorage = new List<GameObject>();
    }

    // Update is called once per frame
    public static void UpdateStorage()
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

    public void AddBlock(GameObject block)
    {
        blockStorage.Add(block);
    }
}
