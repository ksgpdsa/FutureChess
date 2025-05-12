using Enums;
using Pieces;
using UnityEngine;

[System.Serializable]
public class HouseData : MonoBehaviour
{
    public int line;
    public int ring;
    public AreaTypeEnum owner; // "Player1", "Neutral", "Player2"
    public Piece occupant; // quem está ocupando (null se vazio)
    
    private bool _isClickable;
    
    private void OnMouseDown()
    {
        if (_isClickable)
        {
            // Notifica o GameManager (ou outro controlador)
            BoardManager.Instance.OnHouseClicked(this);
        }
    }

    public void ClickByPiece()
    {
        if (_isClickable)
        {
            BoardManager.Instance.OnHouseClicked(this);
        }
    }

    public void SetIsClickable(bool isClickable)
    {
        _isClickable = isClickable;
    }
}