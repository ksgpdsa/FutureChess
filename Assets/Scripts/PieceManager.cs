using Pieces;
using UnityEngine;

namespace DefaultNamespace
{
    public class PieceManager
    {
        public int KingCount;
        public int QueenCount;
        public int KnightCount;
        public int RookCount;
        public int BishopCount;
        public int PawnCount;
        public readonly int Count;

        public PieceManager(int kingCount, int queenCount, int knightCount, int rookCount, int bishopCount, int pawnCount)
        {
            KingCount = kingCount;
            QueenCount = queenCount;
            KnightCount = knightCount;
            RookCount = rookCount;
            BishopCount = bishopCount;
            PawnCount = pawnCount;
            Count = kingCount + queenCount + knightCount + rookCount + bishopCount + pawnCount;
        }
    }
}