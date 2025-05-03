using System.Collections.Generic;using System.Linq;using Enums;using UnityEngine;

namespace DatabasesInternal
{
    [CreateAssetMenu(fileName = "PieceDatabase", menuName = "ScriptableObjects/PieceDatabase", order = 1)]
    public class PieceDatabase : ScriptableObject
    {
        [System.Serializable]
        public class PieceEntry
        {
            public PieceTypeEnum type;
            public GameObject prefab;
        }

        public List<PieceEntry> pieces;

        public GameObject GetPrefab(PieceTypeEnum type)
        {
            foreach (var entry in pieces.Where(entry => entry.type == type))
            {
                return entry.prefab;
            }

            Debug.LogWarning($"Peça do tipo {type} não encontrada no banco de dados!");
            return null;
        }
    }
}