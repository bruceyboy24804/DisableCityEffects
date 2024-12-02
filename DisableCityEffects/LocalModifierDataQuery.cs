using Colossal.Mathematics;
using Game;
using Game.Buildings;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using System.Collections.Generic;
using Unity.Mathematics;
using Colossal.Entities;
using Colossal.Logging;

namespace DisableCityEffects
{
    public partial class LocalModifierDataQuery : GameSystemBase
    {
        ILog m_log;
        private EntityQuery m_LocalModifierDataQuery;
        PrefabSystem m_PrefabSystem;
        internal static object instance;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_LocalModifierDataQuery = GetEntityQuery(ComponentType.ReadWrite<LocalModifierData>());
            RequireForUpdate(m_LocalModifierDataQuery);
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        public partial struct ModifyLocalModifierDataJob : IJobEntity
        {
            public Bounds1 newDelta;
            public Bounds1 newRadius;

            void Execute(ref DynamicBuffer<LocalModifierData> buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    var modifierData = buffer[i];
                    modifierData.m_Delta = newDelta;
                    modifierData.m_Radius = newRadius;
                    buffer[i] = modifierData;
                }
            }
        }

        protected override void OnUpdate()
        {
            



        }
        public void SetModifierDataToZero()
        {
            NativeArray<Entity> instanceEntities = m_LocalModifierDataQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in instanceEntities)
            {
                if (EntityManager.TryGetBuffer(entity, isReadOnly: false, out DynamicBuffer<LocalModifierData> localModifierBuffer))
                {
                    for (int i = 0; i < localModifierBuffer.Length; i++)
                    {
                        LocalModifierData localModifierData = localModifierBuffer[i];
                        localModifierData.m_Delta = new Bounds1(0, 0);
                        localModifierBuffer[i] = localModifierData;
                    }
                }
            }

            m_log.Info($"{nameof(LocalModifierDataQuery)}.{nameof(SetModifierDataToZero)} Complete.");

            instanceEntities.Dispose();
        }
        public void ResetLocalModifierData()
        {
            NativeArray<Entity> prefabEntities = m_LocalModifierDataQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in prefabEntities)
            {
                if (EntityManager.TryGetBuffer(entity, isReadOnly: false, out DynamicBuffer<LocalModifierData> localModifierBuffer)
                    && m_PrefabSystem.TryGetPrefab<PrefabBase>(entity, out var prefab)
                    && EntityManager.TryGetBuffer(m_PrefabSystem.GetEntity(prefab), isReadOnly: true, out DynamicBuffer<LocalModifierData> prefabBuffer))
                {
                    for (int i = 0; i < localModifierBuffer.Length && i < prefabBuffer.Length; i++)
                    {
                        localModifierBuffer[i] = prefabBuffer[i];
                    }
                }
            }

            m_log.Info($"{nameof(LocalModifierDataQuery)}.{nameof(ResetLocalModifierData)} Complete.");
            prefabEntities.Dispose();
        }
    }
}
