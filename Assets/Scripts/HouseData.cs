using Enums;using Pieces;
using UnityEngine;

[System.Serializable]
public class HouseData : MonoBehaviour
{
    public int line;
    public int ring;
    public AreaTypeEnum owner; // "Player1", "Neutral", "Player2"
    public Piece occupant; // quem está ocupando (null se vazio)
}