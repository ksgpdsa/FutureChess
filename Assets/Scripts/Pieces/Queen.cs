using System.Collections.Generic;
using UnityEngine;

namespace Pieces
{
    public class Queen : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            var validMoves = new List<Vector2Int>();

            var directions = new Vector2Int[]
            {
                new(1, 0),   // Cima (próximo anel)
                new(-1, 0),  // Baixo (anel anterior)
                new(0, 1),   // Avança no sentido horário (casa +1)
                new(0, -1),  // Avança no sentido anti-horário (casa -1)

                new(1, 1),   // Diagonal: cima + horário
                new(1, -1),  // Diagonal: cima + anti-horário
                new(-1, 1),  // Diagonal: baixo + horário
                new(-1, -1), // Diagonal: baixo + anti-horário
            };

            foreach (var dir in directions)
            {
                while (true)
                {
                    currentPosition += dir;

                    if (!BoardManager.Instance.IsPositionInsideBoard(currentPosition)) break;

                    if (!BoardManager.Instance.IsValidMove(this, currentPosition)) break;

                    validMoves.Add(currentPosition);

                    // Se tiver peça no caminho, para
                    var pieceAtTarget = BoardManager.Instance.GetPieceAt(currentPosition);
                    if (pieceAtTarget && pieceAtTarget.owner != owner) break;
                }
            }

            return validMoves;
        }
    }
}