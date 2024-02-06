using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PawnChanger : MonoBehaviour
{
    PrefabAssetType prefab;
    [SerializeField] Dictionary<bool, PrefabAssetType> chessType = new Dictionary<bool, PrefabAssetType>();
    [SerializeField] List<string> chess;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
