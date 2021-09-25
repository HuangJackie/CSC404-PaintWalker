using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonTrigger : MonoBehaviour
{
    public bool isTriggered = false;
    public MoveWall wall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            wall.operate = true;
            isTriggered = false;
        }
    }

    void TriggerButtton()
    {
        isTriggered = true;
    }
}
