namespace Pieces
{
    public class Queen : Piece
    {
        protected override void ManageMovementsByType()
        {
            MaxRingForward = 99;
            MaxRingBackward = 99;
            MaxLineForward = 99;
            MaxLineBackward = 99;
            MaxDiagonalSteps = 99;
        }
    }
}