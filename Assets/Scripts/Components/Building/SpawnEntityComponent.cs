using Components.Decorators;
using Unity.Entities;

namespace Components {
    public struct SpawnEntityComponent : IComponentData, IRemoveForPreviewEntity {
        public enum Mode : byte {
            /// <summary>
            /// A random entityPrefab is selected
            /// </summary>
            Random,
            /// <summary>
            /// Prefabs are selected one after another. Once the list of prefabs is finished, startover
            /// </summary>
            RoundRobin
        }
        
        /// <summary>
        /// The mode this entity spawner operates
        /// </summary>
        public Mode mode;

        /// <summary>
        /// The last idx from attached prefabs-buffer
        /// </summary>
        public byte lastSelectedIdx;
    }

    public struct  SpawnEntityPrefabBufferElement : IBufferElementData {
        /// <summary>
        /// entityPrefab that can be spawned by the attached SpanwEntity-Logic
        /// </summary>
        public Entity prefabEntity;
    }
}