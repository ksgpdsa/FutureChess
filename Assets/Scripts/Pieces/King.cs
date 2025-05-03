using System.Collections.Generic;
using UnityEngine;

namespace Pieces
{
    public class King : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            // var currentHouse = BoardManager.Instance.GetHouse(currentPosition.x, currentPosition.y);

            return GetMovesFromOffset(1, 1, 1, 1, 1);
        }
    }
}