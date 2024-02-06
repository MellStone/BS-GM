using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChessList", menuName = "ScriptableObjects/ChessListSO", order = 1)]
public class ChessListSO : ScriptableObject
{
    [SerializeField] public Dictionary<bool, string> chessType = new Dictionary<bool, string>();
}
