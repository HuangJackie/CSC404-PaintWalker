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
    private Tilemap _tileMap;
    //private Vector3 _spawnLocation = new Vector3(2, 0, -2);
    private Vector3Int _spawnLocation = new Vector3Int(2, -3, 1);
    void Start()
    {
        //_spawnedBlock = GameObject.Instantiate(Resources.Load(path: "GreenSpawn")) as GameObject;
        //_spawnedBlock.transform.position = _spawnLocation;
        // _tileMap.SetTile(_spawnLocation, Resources.Load(path: "GreenSpawn"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawn_tile()
    {
        //Instantiate();
    }
}
