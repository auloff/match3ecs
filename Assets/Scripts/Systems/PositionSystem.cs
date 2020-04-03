using Match3.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    [UpdateAfter(typeof(SpawnerSystem))]
    public class PositionSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct PositionJob : IJobForEach<CellPositionComponent, Translation>
        {
            public float DeltaTime;
            public float Speed;

            public int Width;
            public int Height;

            public void Execute([ReadOnly] ref CellPositionComponent position, ref Translation translation)
            {
                var newPosition = new float3(position.x - Width / 2.0f + 0.5f, position.y - Height / 2.0f + 0.5f, 0);

                if (justSpawned.Value == 0)
                {
                    var direction = newPosition - translation.Value;
                    if (math.length(direction) > 0.5f)
                    {
                        var delta = math.normalize(direction) * Speed * DeltaTime;
                        translation.Value += delta;                    
                    }
                    else
                    {
                        translation.Value = newPosition;
                    }                    
                }
                else
                {
                    newPosition += new float3(0, Height, 0);
                    translation.Value = newPosition;
                    justSpawned.Value = 0;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var setting = GetSingleton<GameBoardSettingsComponent>();
            return new PositionJob
            {
                DeltaTime = Time.DeltaTime,
                Speed = setting.CellSpeed,
                Width = setting.Width,
                Height = setting.Height
            }.Schedule(this, inputDeps);
        }
    }
}