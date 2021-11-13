using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    RaycastHit hitInfo;
    LayerMask mask;
    private bool active;
    private CameraRotation cameraPanningRevertTarget;


    private void Start()
    {
        mask = LayerMask.GetMask("Player");
        cameraPanningRevertTarget = FindObjectOfType<CameraRotation>();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Debug.DrawRay(this.transform.position, Vector3.up * 2, Color.black);
            if (Physics.Raycast(this.transform.position, Vector3.up, out hitInfo, 2, mask))
            {
                LevelManager.checkpointInfo["checkpointPos"] = this.transform.position;
                LevelManager.pastCheckPoints.Add(this.transform.position);
                LevelManager.checkpointInfo["playerRotation"] = hitInfo.transform.rotation;
                LevelManager.checkpointInfo["cameraAttributes"] = cameraPanningRevertTarget;
                ObjectStorage.UpdateStorage();
                active = false;
            }
        }
        
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    print("this");
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        if (LevelManager.checkpointPos == Vector3.zero)
    //        {
    //            LevelManager.checkpointPos = this.transform.position;
    //            LevelManager.pastCheckPoints.Add(this.transform.position);
    //        } else if (!LevelManager.pastCheckPoints.Contains(this.transform.position))
    //        {
    //            LevelManager.checkpointPos = this.transform.position;
    //            LevelManager.pastCheckPoints.Add(this.transform.position);
    //        }
    //    }
    //}
}
