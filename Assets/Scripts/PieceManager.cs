using UI;

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
        public int Count;

        public PieceManager(int kingCount, int queenCount, int knightCount, int rookCount, int bishopCount, int pawnCount)
        {
            SetKingCount(kingCount);
            SetQueenCount(queenCount);
            SetKnightCount(knightCount);
            SetRookCount(rookCount);
            SetBishopCount(bishopCount);
            SetPawnCount(pawnCount);
        }

        public void SetKingCount(int kingCount)
        {
            KingCount = kingCount;
            HudManager.Instance.SetKingCount(kingCount);
            UpdateTotalCount();
        }

        public void SetQueenCount(int queenCount)
        {
            QueenCount = queenCount;
            HudManager.Instance.SetQueenCount(queenCount);
            UpdateTotalCount();
        }

        public void SetKnightCount(int knightCount)
        {
            KnightCount = knightCount;
            HudManager.Instance.SetKnightCount(knightCount);
            UpdateTotalCount();
        }

        public void SetRookCount(int rookCount)
        {
            RookCount = rookCount;
            HudManager.Instance.SetRookCount(rookCount);
            UpdateTotalCount();
        }

        public void SetBishopCount(int bishopCount)
        {
            BishopCount = bishopCount;
            HudManager.Instance.SetBishopCount(bishopCount);
            UpdateTotalCount();
        }

        public void SetPawnCount(int pawnCount)
        {
            PawnCount = pawnCount;
            HudManager.Instance.SetPawnCount(pawnCount);
            UpdateTotalCount();
        }

        private void UpdateTotalCount()
        {
            Count = KingCount + QueenCount + KnightCount + RookCount + BishopCount + PawnCount;
        }
    }
}