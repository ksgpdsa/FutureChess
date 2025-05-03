using System.Collections.Generic;using Enums;
using UnityEngine;

namespace Pieces
{
    public class Rook : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            var validMoves = new List<Vector2Int>();
            var myArea = BoardManager.Instance.GetAreaType(currentPosition);

            // Direções: cima (anel +1), baixo (anel -1), direita (casa +1), esquerda (casa -1)
            var directions = new Vector2Int[]
            {
                new(1, 0),  // cima (anéis para fora)
                new(-1, 0), // baixo (anéis para dentro)
                new(0, 1),  // direita (próxima casa no mesmo anel)
                new(0, -1)  // esquerda (casa anterior no mesmo anel)
            };

            foreach (var dir in directions)
            {
                while (true)
                {
                    currentPosition += dir;

                    if (!BoardManager.Instance.IsPositionInsideBoard(currentPosition))
                        break;

                    // Verificar área atual do movimento
                    var targetArea = BoardManager.Instance.GetAreaType(currentPosition);

                    // Dentro da sua área, pode andar livremente
                    // TODO: mudar para área do jogador
                    if (myArea == AreaTypeEnum.Player1 && targetArea == AreaTypeEnum.Player1)
                    {
                        // Parar se encontrar qualquer peça
                    }
                    else
                    {
                        // Fora da sua área: só movimentação horizontal (casa ±1)
                        if (dir.x != 0) break; // Tentou mudar de anel? Não pode, só casa ±1!
                    }

                    if (!BoardManager.Instance.IsValidMove(this, currentPosition)) break;
                    
                    validMoves.Add(currentPosition);
                    
                    if (BoardManager.Instance.GetPieceAt(currentPosition)) break; // Parar se encontrar qualquer peça
                }
            }

            return validMoves;
        }

        public override void Move(Vector2Int newPosition)
        {
            base.Move(newPosition);

            var areaType = BoardManager.Instance.GetAreaType(newPosition);
            
            // A casa que a torre pisa vira da área dela
            BoardManager.Instance.SwitchOwner(newPosition, areaType);
        }
    }
}
