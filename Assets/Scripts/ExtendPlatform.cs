using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

public class ExtendPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _spawnedBlock;
    private Vector3 _spawnLocation = new Vector3(2.5f, 0f, -2.5f);
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawn_tile()
    {
        _spawnedBlock = GameObject.Instantiate(Resources.Load(path: "GreenSpawn")) as GameObject;
        _spawnedBlock.transform.position = _spawnLocation;
    }
}
