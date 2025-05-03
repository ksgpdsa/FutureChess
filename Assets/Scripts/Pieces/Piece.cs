using UnityEngine;using System.Collections.Generic;
using System.Linq;
using Enums;


namespace Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public Vector2Int currentPosition; // Ex: (Anel, PosiçãoNoAnel)
        public PlayerEnum owner; // "Player1" ou "Player2"
        public PieceTypeEnum pieceType;

        public abstract List<Vector2Int> GetValidMoves();
        
        protected List<Vector2Int> GetMovesFromOffset(int maxRingForward, int maxRingBackward, int maxLineForward, int maxLineBackward, int maxDiagonalSteps = 0)
        {
            var validMoves = new List<Vector2Int>();
            var houses = BoardManager.Instance.GetHouses();

            foreach (var house in houses)
            {
                var lineDiference = house.line - currentPosition.x; // diferença de linha
                var ringDiference = house.ring - currentPosition.y; // diferença de anel (ring)
                var houseQuantity = BoardManager.Instance.GetRing(house.ring).houseQuantity;

                if (lineDiference == 0 && ringDiference == 0) continue;

                // Frente/trás = mover no eixo do ring (y)
                if (lineDiference == 0 && ringDiference > 0 && ringDiference <= maxRingForward)
                {
                    validMoves.Add(new Vector2Int(house.line, house.ring));
                    continue;
                }
                
                if (lineDiference == 0 && ringDiference < 0 && -ringDiference <= maxRingBackward)
                {
                    validMoves.Add(new Vector2Int(house.line, house.ring));
                    continue;
                }

                // Lados = mover no eixo da linha (x)
                if (ringDiference == 0 && lineDiference > 0 && (lineDiference <= maxLineForward || house.line == houseQuantity))
                {
                    validMoves.Add(new Vector2Int(house.line, house.ring));
                    continue;
                }
                
                if (ringDiference == 0 && lineDiference < 0 && (-lineDiference <= maxLineBackward || (currentPosition.x == houseQuantity && house.line == 1)))
                {
                    validMoves.Add(new Vector2Int(house.line, house.ring));
                    continue;
                }

                // Diagonais
                if (
                    maxDiagonalSteps > 0
                    && Mathf.Abs(lineDiference) == Mathf.Abs(ringDiference) 
                    && (
                        Mathf.Abs(lineDiference) <= maxDiagonalSteps 
                        || house.line == houseQuantity 
                        || (currentPosition.x == 1 && house.line == houseQuantity) 
                        || (currentPosition.x == houseQuantity && house.line == 1)
                    )
                )
                {
                    validMoves.Add(new Vector2Int(house.line, house.ring));
                }
                else if (house.ring == 2 && house.line == 16)
                {
                    Debug.Log("B > " + (Mathf.Abs(lineDiference)));
                    Debug.Log("B > " + (Mathf.Abs(ringDiference)));
                    
                    // Debug.Log("Diagonais(current): " + currentPosition.x + ", " + currentPosition.y);
                    // Debug.Log("Diagonais(houseQuantity): " + houseQuantity);
                    // Debug.Log("Diagonais(houseLine): " + house.line + ", RING " + house.ring);
                }
            }

            foreach (var validMove in validMoves)
            {
                Debug.LogWarning(validMove);
            }

            return validMoves;
        }


        public virtual void Move(Vector2Int newPos)
        {
            transform.position = BoardManager.Instance.GetWorldPosition(newPos);
            currentPosition = newPos;
        }
    }

}