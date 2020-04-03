using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using Match3.ECS.Components;

namespace Match3.ECS.Helpers
{
    public class SettingsToEntityConverter : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        [SerializeField]
        private int _width = 15;
        public int Width { get => _width; }

        [SerializeField]
        private int _height = 15;
        public int Height { get => _height; }

        [SerializeField]
        private int _cellSpeed = 30;
        public int CellSpeed { get => _cellSpeed; }

        [SerializeField]
        private int _groupSize = 3;
        public int GroupSize { get => _groupSize; }

        public GameObject[] Prefabs;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GameBoardSettingsComponent { 
                Width = _width,
                Height = _height,
                CellSpeed = _cellSpeed,
                SetSize = Prefabs.Length,
                MinGroupSize = _groupSize
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.AddRange(Prefabs);
        }
    }
}