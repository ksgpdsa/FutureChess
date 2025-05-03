using System.Collections.Generic;using Enums;
using UnityEngine;

namespace Pieces
{
    public class Pawn : Piece
    {
        public override List<Vector2Int> GetValidMoves()
        {
            var validMoves = new List<Vector2Int>();

            var myArea = BoardManager.Instance.GetAreaType(currentPosition); // área atual (Player1, Player2, Neutral)

            var forwardDirection = (owner == PlayerEnum.Player1) ? 1 : -1; // direção de avanço no eixo do anel (ex: "pra frente" é aumentar ou diminuir anel)

            // Determina quantos anéis o peão pode avançar
            var maxStep = (myArea == AreaTypeEnum.Player1 && owner == PlayerEnum.Player1) ||
                          (myArea == AreaTypeEnum.Player2 && owner == PlayerEnum.Player2)
                          ? 2 : 1;

            for (var step = 1; step <= maxStep; step++)
            {
                var target = new Vector2Int(currentPosition.x, currentPosition.y + forwardDirection * step);

                if (!BoardManager.Instance.IsPositionInsideBoard(target)) continue;
                
                if (!BoardManager.Instance.GetPieceAt(target))
                {
                    validMoves.Add(target);
                }
                else
                {
                    // Peão não pula peça
                    break;
                }
            }

            // TODO: captura de peças inimigas (se quiser depois)

            return validMoves;
        }

        public void PromoteTo(PieceTypeEnum newTypeEnum)
        {
            Debug.Log($"Promovendo peão para {newTypeEnum}");

            switch (newTypeEnum)
            {
                case PieceTypeEnum.Queen:
                    gameObject.AddComponent<Queen>();
                    break;
                case PieceTypeEnum.Rook:
                    gameObject.AddComponent<Rook>();
                    break;
                case PieceTypeEnum.Bishop:
                    gameObject.AddComponent<Bishop>();
                    break;
                case PieceTypeEnum.Knight:
                    gameObject.AddComponent<Knight>();
                    break;
            }

            Destroy(this); // Remove o script de peão (agora virou outra peça)
        }
    }
}
