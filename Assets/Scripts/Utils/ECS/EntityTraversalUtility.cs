using System;
using System.Collections.Generic;
using Unity.Entities;

// The visitor function signature: returns true if the entity should be included in the result list
public delegate bool EntityVisitor(Entity entity, EntityManager entityManager);

public static class EntityTraversalUtility {
    /// <summary>
    /// Traverses the given entity and its children (via LinkedEntityGroup), applies the visitor function,
    /// and returns a list of entities for which the visitor returned true.
    /// </summary>
    public static List<Entity> TraverseEntityHierarchy(
        Entity root,
        EntityManager entityManager,
        EntityVisitor visitor) {
        var result = new List<Entity>();
        var visited = new HashSet<Entity>();
        TraverseRecursive(root, entityManager, visitor, result, visited);
        return result;
    }

    private static void TraverseRecursive(
        Entity entity,
        EntityManager entityManager,
        EntityVisitor visitor,
        List<Entity> result,
        HashSet<Entity> visited) {
        if (!visited.Add(entity))
            return; // Prevent cycles

        if (visitor(entity, entityManager))
            result.Add(entity);

        // Traverse children if any
        if (entityManager.HasComponent<LinkedEntityGroup>(entity)) {
            var buffer = entityManager.GetBuffer<LinkedEntityGroup>(entity);
            // The first element is the parent itself, so skip it
            for (int i = 1; i < buffer.Length; i++) {
                TraverseRecursive(buffer[i].Value, entityManager, visitor, result, visited);
            }
        }
    }
}
