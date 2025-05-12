namespace Pieces
{
    public class King : Piece
    {
        protected override void ManageMovementsByType()
        {
            MaxRingForward = 1;
            MaxRingBackward = 1;
            MaxLineForward = 1;
            MaxLineBackward = 1;
            MaxDiagonalSteps = 1;
        }
    }
}