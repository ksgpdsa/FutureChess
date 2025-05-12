using System.Collections.Generic;
using UnityEngine;

namespace Pieces
{
    public class Knight : Piece
    {
        protected override void ManageMovementsByType()
        {
            MaxRingForward = 2;
            MaxRingBackward = 2;
            MaxLineForward = 2;
            MaxLineBackward = 2;
        }
        
        public override List<Vector2Int> GetValidMoves()
        {
            currentHouse = BoardManager.Instance.GetHouse(currentPosition.x, currentPosition.y);
            var validMoves = new List<Vector2Int>();
            
            var offsets = new List<(int dr, int dl)>
            {
                (2, 1), (2, -1), (-2, 1), (-2, -1),
                (1, 2), (-1, 2), (1, -2), (-1, -2)
            };

            foreach (var (deltaRing, deltaLine) in offsets)
            {
                var targetRing = currentHouse.ring + deltaRing;

                var nextRingData = BoardManager.Instance.GetRing(targetRing);
                var currentRingData = BoardManager.Instance.GetRing(currentHouse.ring);

                if (nextRingData == null || currentRingData == null)
                {
                    continue;
                }

                var currentQty = currentRingData.houseQuantity;
                var nextQty = nextRingData.houseQuantity;

                var finalLine = FixToCircularMove(deltaLine, currentHouse.line, currentQty, nextQty);

                if (finalLine < 1)
                {
                    finalLine += nextQty;
                }
                else if (finalLine > nextQty)
                {
                    finalLine -= nextQty;
                }

                var house = BoardManager.Instance.GetHouse(finalLine, targetRing);
                
                if (house && (!house.occupant || house.occupant.owner != owner))
                {
                    house.SetIsClickable(true);
                    
                    validMoves.Add(new Vector2Int(house.line, house.ring));
                }
            }

            return validMoves;
        }
    }
}