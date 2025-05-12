namespace Pieces
{
    public class Bishop : Piece
    {
        protected override void ManageMovementsByType()
        {
            MaxDiagonalSteps = 99;
        }

        protected override bool ManageRingMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return false; // não move em linha reta
        }

        protected override bool ManageLineMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return false; // não move em linha reta
        }

        protected override bool ManageDiagonalMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            if (HasCrossedTooFarIntoAnotherOwner(targetHouse, ref crossedOwnerBoundary))
            {
                return steps == 1; // fora da área: 1 anel por vez
            }
            
            return true;
        }
    }
}
