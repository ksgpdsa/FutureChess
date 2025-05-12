using Enums;
using UnityEngine;

namespace Pieces
{
    public class Pawn : Piece
    {
        protected override void ManageMovementsByType()
        {
            MaxRingForward = 2;
            MaxDiagonalSteps = 1;
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
        
        protected override bool ManageRingMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            if (targetHouse.owner == (AreaTypeEnum)owner)
            {
                // Na sua área: pode pular 2 anéis
                return steps <= 2;
            }

            // Fora da área: só 1 anel por vez
            return steps == 1;
        }

        protected override bool ManageLineMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            // Peão não se move lateralmente
            return false;
        }

        protected override bool ManageDiagonalMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            // Só pode capturar se for inimigo
            return targetHouse.occupant && targetHouse.occupant.owner != owner;
        }
    }
}
