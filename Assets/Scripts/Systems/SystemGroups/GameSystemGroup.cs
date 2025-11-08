using Unity.Entities;

namespace Systems.SystemGroups {
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial class GameSystemGroup : ComponentSystemGroup {

    }

    //// BEGIN
    //[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    //public partial class BeginFrameBufferSystem : EntityCommandBufferSystem { }

    //// END
    //[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    //public partial class EndFrameBufferSystem : EntityCommandBufferSystem { }
}