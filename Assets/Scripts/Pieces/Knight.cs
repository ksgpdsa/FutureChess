using System.Collections.Generic;using UnityEngine;

namespace Pieces
{
    public class Knight : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            var moves = new List<Vector2Int>();

            int[] anelMoves = { -2, -1, 1, 2 };
            int[] posMoves = { -2, -1, 1, 2 };

            foreach (var a in anelMoves)
            {
                foreach (var p in posMoves)
                {
                    if (Mathf.Abs(a) == Mathf.Abs(p)) continue; // Movimentos em L
                    
                    var newPos = new Vector2Int(currentPosition.x + a, currentPosition.y + p);
                    
                    if (BoardManager.Instance.IsValidMove(this, newPos))
                    {
                        moves.Add(newPos);
                    }
                }
            }

            return moves;
        }
    }
}