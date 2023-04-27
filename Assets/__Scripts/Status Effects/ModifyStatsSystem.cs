/*using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TMG.StatBuffs
{
    public partial class ModifyStatsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            Entities
                .WithAll<ModifyStatsTag>()
                .ForEach((Entity playerEntity, in CharacterStats baseStats, in StatModification statModifier) =>
                {
                    baseStats.Modify(out var modifiedStats, statModifier);
                    modifiedStats.SetAsTotalComponents(ref ecb, playerEntity);
                    ecb.RemoveComponent<ModifyStatsTag>(playerEntity);
                }).Run();

            Entities
                .WithAll<ModifyStatsTag>()
                .WithNone<StatModification>()
                .ForEach((Entity playerEntity, in CharacterStats baseStats) =>
                {
                    baseStats.SetAsTotalComponents(ref ecb, playerEntity);
                    ecb.RemoveComponent<ModifyStatsTag>(playerEntity);
                }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
    
    [BurstCompile]
    public static class StatExtensions
    {
        private delegate float ModificationDelegate(float a, float b);

        [BurstCompile]
        private static float PercentageModification(float baseValue, float percentage)
        {
            return baseValue * percentage * 0.01f;
        }

        private static readonly FunctionPointer<ModificationDelegate> _percentageModificationPointer =
            BurstCompiler.CompileFunctionPointer<ModificationDelegate>(PercentageModification);
        
        [BurstCompile]
        private static float NumericalModification(float baseValue, float numericalValue)
        {
            return baseValue + numericalValue;
        }

        private static readonly FunctionPointer<ModificationDelegate> _numericalModificationPointer =
            BurstCompiler.CompileFunctionPointer<ModificationDelegate>(NumericalModification);
        
        [BurstCompile]
        private static float AbsoluteModification(float baseValue, float absoluteValue)
        {
            return absoluteValue;
        }

        private static readonly FunctionPointer<ModificationDelegate> _absoluteModificationPointer =
            BurstCompiler.CompileFunctionPointer<ModificationDelegate>(AbsoluteModification);
        
        public static void Modify(this in CharacterStats baseStats, out CharacterStats modifiedStats, 
            in StatModification statMod)
        {
            var modOperation = statMod.ModificationType switch
            {
                StatModificationTypes.Percentage => _percentageModificationPointer,
                StatModificationTypes.Numerical => _numericalModificationPointer,
                StatModificationTypes.Absolute => _absoluteModificationPointer,
                _ => throw new ArgumentOutOfRangeException()
            };

            modifiedStats = baseStats;
            
            switch (statMod.StatToModify)
            {
                case StatTypes.AttackDamage:
                    modifiedStats.AttackDamage = modOperation.Invoke(baseStats.AttackDamage, statMod.ModificationValue);
                    break;

                case StatTypes.MoveSpeed:
                    modifiedStats.MoveSpeed = modOperation.Invoke(baseStats.MoveSpeed, statMod.ModificationValue);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Not Burst Compiled
        public static void SetAsTotalComponents(this in CharacterStats characterStats, ref EntityCommandBuffer ecb,
            in Entity characterEntity)
        {
            var attackDamage = new TotalAttackDamage { Value = characterStats.AttackDamage };
            ecb.SetComponent(characterEntity, attackDamage);

            var moveSpeed = new TotalMoveSpeed { Value = characterStats.MoveSpeed };
            ecb.SetComponent(characterEntity, moveSpeed);
        }
    }
}*/