using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBob : MonoBehaviour
{
    private Vector3 neutralPos;
    private Vector3 topBobPos;
    private Vector3 botBobPos;
    private bool goingUp = true;
    // Start is called before the first frame update
    void Start()
    {
        neutralPos = this.transform.position;
        topBobPos = neutralPos + Vector3.up * 0.2f;
        botBobPos = neutralPos + Vector3.down * 0.2f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (goingUp)
        {
            transform.position = Vector3.MoveTowards(
                    transform.position, topBobPos, 0.2f * Time.deltaTime
            );
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                    transform.position, botBobPos, 0.2f * Time.deltaTime
            );
        }
        if (goingUp && Vector3.Distance(transform.position, topBobPos) <= 0.01f)
        {
            transform.position = topBobPos;
            goingUp = false;
        }
        if (!goingUp && Vector3.Distance(transform.position, botBobPos) <= 0.01f)
        {
            transform.position = botBobPos;
            goingUp = true;
        }
    }

}
