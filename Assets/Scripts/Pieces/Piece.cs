using Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public Vector2Int currentPosition; // (line, ring)
        public HouseData currentHouse;
        public PlayerEnum owner;
        public PieceTypeEnum pieceType;

        protected int MaxRingForward;
        protected int MaxRingBackward;
        protected int MaxLineForward;
        protected int MaxLineBackward;
        protected int MaxDiagonalSteps;

        public virtual List<Vector2Int> GetValidMoves()
        {
            var validMoves = new List<Vector2Int>();

            ResetClickableHouses();

            ManageMovementsByType();
            
            AddMoves(validMoves);

            CentralMoves(validMoves);

            return validMoves;
        }

        private void ResetClickableHouses()
        {
            var allHouses = BoardManager.Instance.GetHouses();

            foreach (var house in allHouses)
            {
                house.SetIsClickable(false);
            }
        }

        private void CentralMoves(List<Vector2Int> validMoves)
        {
            // Movimentos especiais da casa central
            if (currentHouse.ring == 0 && currentHouse.line == 1)
            {
                var firstRing = BoardManager.Instance.GetRing(1);
               
                for (var line = 1; line <= firstRing.houseQuantity; line++)
                {
                    var house = BoardManager.Instance.GetHouse(line, 1);
                    if (house && (!house.occupant || house.occupant.owner != owner))
                    {
                        validMoves.Add(new Vector2Int(line, 1));
                    }
                }
            }

            // Retorno para casa central
            if (currentHouse.ring == 1 && MaxRingBackward > 0)
            {
                var center = BoardManager.Instance.GetHouse(1, 0);
                
                if (center && (!center.occupant || center.occupant.owner != owner))
                {
                    validMoves.Add(new Vector2Int(1, 0));
                }
            }
        }

        private void AddMoves(List<Vector2Int> validMoves)
        {
            if (MaxRingForward > 0)
            {
                AddMovesInDirection(validMoves, 1, 0, MaxRingForward, DirectionsEnum.Vertical);
            }

            if (MaxRingBackward > 0)
            {
                AddMovesInDirection(validMoves, -1, 0, MaxRingBackward, DirectionsEnum.Vertical);
            }

            if (MaxLineForward > 0)
            {
                AddMovesInDirection(validMoves, 0, 1, MaxLineForward, DirectionsEnum.Horizontal);
            }

            if (MaxLineBackward > 0)
            {
                AddMovesInDirection(validMoves, 0, -1, MaxLineBackward, DirectionsEnum.Horizontal);
            }

            if (MaxDiagonalSteps > 0)
            {
                AddMovesInDirection(validMoves, 1, 1, MaxDiagonalSteps, DirectionsEnum.Diagonal);
                AddMovesInDirection(validMoves, 1, -1, MaxDiagonalSteps, DirectionsEnum.Diagonal);
                AddMovesInDirection(validMoves, -1, 1, MaxDiagonalSteps, DirectionsEnum.Diagonal);
                AddMovesInDirection(validMoves, -1, -1, MaxDiagonalSteps, DirectionsEnum.Diagonal);
            }
        }

        private void AddMovesInDirection(List<Vector2Int> validMoves, int deltaRing, int deltaLine, int maxSteps, DirectionsEnum direction)
        {
            var steps = 0;
            var ring = currentHouse.ring;
            var line = currentHouse.line;
            var crossedOwnerBoundary = false;

            if (owner == PlayerEnum.Player2)
            {
                deltaRing *= -1;
            }

            while (steps < maxSteps)
            {
                var prevRing = ring;
                ring += deltaRing;
                steps++;

                var nextRingData = BoardManager.Instance.GetRing(ring);
                
                if (nextRingData == null)
                {
                    break;
                }

                var prevRingData = BoardManager.Instance.GetRing(prevRing);
                
                if (prevRingData == null)
                {
                    break;
                }

                var prevQty = prevRingData.houseQuantity;
                var nextQty = nextRingData.houseQuantity;

                if (deltaRing == 0) // Horizontal (mesmo anel)
                {
                    line += deltaLine;
                    
                    if (line < 1)
                    {
                        line = nextQty;
                    }
                    else if (line > nextQty)
                    {
                        line = 1;
                    }
                }
                else if (deltaLine == 0) // Vertical (mesma linha lógica)
                {
                    var ratio = (float)line / prevQty;
                    var logicalLine = ratio * nextQty;

                    // Só continua se cair exatamente em uma casa ou for a casa central
                    if (nextQty > 1 && !Mathf.Approximately(logicalLine % 1f, 0f))
                    {
                        break; // não marca nenhuma casa se a posição não é exata
                    }

                    line = Mathf.Clamp(Mathf.RoundToInt(logicalLine), 1, nextQty);
                }
                else // Diagonal
                {
                    if (prevQty == nextQty)
                    {
                        // Mesmo número de casas: só aplicar deltaLine com wrap
                        line += deltaLine;

                        if (line < 1)
                        {
                            line = nextQty;
                        }
                        else if (line > nextQty)
                        {
                            line = 1;
                        }
                    }
                    else
                    {
                        if (prevQty > nextQty && line % 2 == 0)
                        {
                            break; // casas pares não se movem na diagonal para anéis com qty diferente
                        }
                        
                        var targetLine = FixToCircularMove(deltaLine, line, prevQty, nextQty);
                        
                        // Movimento circular
                        if (targetLine < 1)
                        {
                            targetLine = nextQty;
                        }
                        else if (targetLine > nextQty)
                        {
                            targetLine = 1;
                        }

                        line = targetLine;
                    }
                }

                if (line == currentHouse.line && ring == currentHouse.ring)
                {
                    break;
                }

                var house = BoardManager.Instance.GetHouse(line, ring);
                
                if (!house)
                {
                    break;
                }

                if (!IsMovementAllowed(house, steps, direction, ref crossedOwnerBoundary)) break;

                house.SetIsClickable(true);
                
                validMoves.Add(new Vector2Int(house.line, house.ring));
            }
        }

        protected static int FixToCircularMove(int deltaLine, int line, int prevQty, int nextQty)
        {
            var ratio = (float)line / prevQty;
            var logicalLine = ratio * nextQty + deltaLine;

            var useTrunc = prevQty > nextQty && deltaLine > 0;

            var targetLine = useTrunc ? Mathf.FloorToInt(logicalLine) : Mathf.CeilToInt(logicalLine);

            return targetLine;
        }

        private bool IsMovementAllowed(HouseData house, int steps, DirectionsEnum direction, ref bool crossedOwnerBoundary)
        {
            if (house.occupant && house.occupant.owner == owner)
            {
                return false;
            }

            return direction switch
            {
                DirectionsEnum.Vertical => ManageRingMovement(house, steps, ref crossedOwnerBoundary),
                DirectionsEnum.Horizontal => ManageLineMovement(house, steps, ref crossedOwnerBoundary),
                DirectionsEnum.Diagonal => ManageDiagonalMovement(house, steps, ref crossedOwnerBoundary),
                _ => true
            };
        }

        protected abstract void ManageMovementsByType();

        protected virtual bool ManageRingMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return true; // padrão: sem restrição
        }

        protected virtual bool ManageLineMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return true; // padrão: sem restrição
        }

        protected virtual bool ManageDiagonalMovement(HouseData targetHouse, int steps, ref bool crossedOwnerBoundary)
        {
            return true; // padrão: sem restrição
        }

        protected bool HasCrossedTooFarIntoAnotherOwner(HouseData targetHouse, ref bool crossedOwnerBoundary)
        {
            // Se o owner for o mesmo, ok
            if (targetHouse.owner == currentHouse.owner) return false;

            // Se ainda não cruzou, permitir uma vez e marcar que cruzou
            if (!crossedOwnerBoundary)
            {
                crossedOwnerBoundary = true;
                return false;
            }

            // Já cruzou antes, agora deve bloquear
            return true;
        }

        public virtual void Move(Vector2Int newPos)
        {
            currentHouse.occupant = null;
            
            transform.position = BoardManager.Instance.GetWorldPosition(newPos, 0.7f);
            currentPosition = newPos;
            
            var house = BoardManager.Instance.GetHouse(currentPosition.x, currentPosition.y);
            house.occupant = this;
            
            currentHouse = house;
            
            ResetClickableHouses();
            
            BoardManager.Instance.ClearHighlights(this);
        }

        public void HasCaptured()
        {
            GameManager.Instance.CapturePiece(owner, pieceType);
            Destroy(gameObject);
        }
    }
}
