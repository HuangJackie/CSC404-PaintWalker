using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public LevelManager _levelManager;


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        print("this");
        if (other.gameObject.CompareTag("Player"))
        {
            if (LevelManager.checkpointPos == Vector3.zero)
            {
                LevelManager.checkpointPos = this.transform.position;
                LevelManager.pastCheckPoints.Add(this.transform.position);
            } else if (!LevelManager.pastCheckPoints.Contains(this.transform.position))
            {
                LevelManager.checkpointPos = this.transform.position;
                LevelManager.pastCheckPoints.Add(this.transform.position);
            }
        }
    }
}
