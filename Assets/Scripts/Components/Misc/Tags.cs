using Unity.Entities;

namespace Components.Tags {
    /// <summary>
    /// Generic Tag to flag entities as working
    /// </summary>
    public struct TagWorking : IComponentData,IEnableableComponent { }

    /// <summary>
    /// Generic Tag to flag entities as begin under construction
    /// </summary>
    public struct TagUnderConstruction : IComponentData, IEnableableComponent { }

    /// <summary>
    /// Generic Tag to flag entities to be built and able to work as intended
    /// </summary>
    public struct TagBuilt : IComponentData, IEnableableComponent { }
}