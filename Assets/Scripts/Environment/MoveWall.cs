using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public bool operate = false;
    private float end_y = -2.5f;
    private Vector3 _targetLocation;
    
    private void Start()
    {
        ObjectStorage.wallStorage.Add(this.gameObject);
        Vector3 currPosition = transform.position;
        _targetLocation = new Vector3(currPosition.x, end_y, currPosition.z);
    }

    void Update()
    {
        if (operate)
        {
            Vector3 pos = Vector3.MoveTowards(
                transform.position, _targetLocation, 1.5f * Time.deltaTime
            );
            
            if (pos.y <= end_y)
            {
                operate = false;
            } else
            {
                transform.position = pos;
            }
        }
    }
}
