using TMPro;
using UnityEngine;

namespace UI
{
    public class HudManager : MonoBehaviour
    {
        public static HudManager Instance;
        
        [SerializeField] private TextMeshProUGUI kingCount;
        [SerializeField] private TextMeshProUGUI queenCount;
        [SerializeField] private TextMeshProUGUI bishopCount;
        [SerializeField] private TextMeshProUGUI rookCount;
        [SerializeField] private TextMeshProUGUI pawnCount;
        [SerializeField] private TextMeshProUGUI knightCount;
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetKingCount(int count)
        {
            kingCount.text = count.ToString();
        }

        public void SetQueenCount(int count)
        {
            queenCount.text = count.ToString();
        }

        public void SetBishopCount(int count)
        {
            bishopCount.text = count.ToString();
        }

        public void SetRookCount(int count)
        {
            rookCount.text = count.ToString();
        }

        public void SetKnightCount(int count)
        {
            knightCount.text = count.ToString();
        }

        public void SetPawnCount(int count)
        {
            pawnCount.text = count.ToString();
        }
    }
}