using System.Collections.Generic;using Enums;
using UnityEngine;

namespace Pieces
{
    public class Bishop : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            var validMoves = new List<Vector2Int>();
            var myArea = BoardManager.Instance.GetAreaType(currentPosition);

            // Diagonais: anel +1 e casa +1, anel +1 e casa -1, anel -1 e casa +1, anel -1 e casa -1
            var directions = new Vector2Int[]
            {
                new(1, 1),   // subir anel e próxima casa
                new(1, -1),  // subir anel e casa anterior
                new(-1, 1),  // descer anel e próxima casa
                new(-1, -1)  // descer anel e casa anterior
            };

            foreach (var dir in directions)
            {
                while (true)
                {
                    currentPosition += dir;

                    if (!BoardManager.Instance.IsPositionInsideBoard(currentPosition)) break;

                    var targetArea = BoardManager.Instance.GetAreaType(currentPosition);

                    // Dentro da sua área: anda livre na diagonal
                    // TODO: mudar para área do jogador
                    if (myArea == AreaTypeEnum.Player1 && targetArea == AreaTypeEnum.Player1)
                    {
                        // Parar se encontrar qualquer peça
                    }
                    else
                    {
                        // Fora da sua área: só pode trocar de anel 1x por movimento
                        if (Mathf.Abs(currentPosition.x - currentPosition.x) > 1) break;
                    }

                    if (!BoardManager.Instance.IsValidMove(this, currentPosition)) break;
                    
                    validMoves.Add(currentPosition);
                    
                    if (BoardManager.Instance.GetPieceAt(currentPosition) != null) break;
                }
            }

            return validMoves;
        }
    }
}
