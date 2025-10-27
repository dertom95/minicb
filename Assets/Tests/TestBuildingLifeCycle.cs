//using System.Collections;
//using Components;
//using Data;
//using Manager;
//using NUnit.Framework;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.TestTools;

//public class TestBuildingLifeCycle : TestHelper
//{
//    //// A Test behaves as an ordinary method
//    //[Test]
//    //public void TestBuildingLifeCycleSimplePasses()
//    //{
//    //    // Use the Assert class to test conditions
//    //}

//    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//    // `yield return null;` to skip a frame.
//    [UnityTest]
//    public IEnumerator TestECS()
//    { 
//        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//        // Use the Assert class to test conditions.
//        // Use yield to skip a frame.
//        yield return LoadDefaultScene();
//        foreach (World world in World.All) {
//            Debug.Log("World:" + world.Name);
//        }

//        Entity gathererEntity = BuildingManager.Instance.SpawnBuilding(Data.BuildingType.gatherer, float3.zero);

//        BuildingComponent buildingComponent = entityManager.GetComponentData<BuildingComponent>(gathererEntity);
//        Assert.AreEqual(BuildingState.UnderConstruction, buildingComponent.currentState);



//        NativeArray<ComponentType> components = entityManager.GetComponentTypes(gathererEntity);
//        int a = 0; 
//    }
//}
