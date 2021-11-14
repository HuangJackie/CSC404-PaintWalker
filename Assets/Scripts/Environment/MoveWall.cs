using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public bool operate = false;
    private float end_y = -0.5f;

    private void Start()
    {
        ObjectStorage.wallStorage.Add(this.gameObject);
    }

    void Update()
    {
        if (operate == true)
        {
            Vector3 pos = transform.position - new Vector3(0, 0.006f, 0);
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
