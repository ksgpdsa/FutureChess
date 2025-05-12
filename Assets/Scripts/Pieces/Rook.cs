using UnityEngine;

namespace Pieces
{
    public class Rook : Piece
    {
        protected override void ManageMovementsByType()
        {
            MaxRingForward = 99;
            MaxRingBackward = 99;
            MaxLineForward = 99;
            MaxLineBackward = 99;
        }
        
        public override void Move(Vector2Int newPosition)
        {
            base.Move(newPosition);

            var house = BoardManager.Instance.GetHouse(newPosition.x, newPosition.y);
            
            // A casa que a torre pisa vira da área dela
            BoardManager.Instance.SwitchOwner(newPosition, house.owner);
        }
        
        protected override bool ManageRingMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            if (HasCrossedTooFarIntoAnotherOwner(targetHouse, ref crossedOwnerBoundary))
            {
                return steps == 1; // fora da área: 1 anel por vez
            }

            return true;
        }

        protected override bool ManageLineMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return true; // sempre pode se mover horizontalmente
        }

        protected override bool ManageDiagonalMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return false; // torre não anda em diagonal
        }
    }
}
