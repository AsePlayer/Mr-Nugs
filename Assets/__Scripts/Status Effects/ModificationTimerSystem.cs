/*using Unity.Collections;
using Unity.Entities;

namespace TMG.StatBuffs
{
    [UpdateAfter(typeof(AddModificationSystem))]
    [UpdateBefore(typeof(ModifyStatsSystem))]
    public partial class ModificationTimerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            Entities
                .WithAll<PlayerTag>()
                .ForEach((Entity characterEntity, ref StatModification statModification) =>
                {
                    statModification.Timer -= deltaTime;
                    if (statModification.Timer <= 0f)
                    {
                        ecb.RemoveComponent<StatModification>(characterEntity);
                        ecb.AddComponent<ModifyStatsTag>(characterEntity);
                    }
                }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}*/