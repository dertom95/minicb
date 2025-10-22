using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Transforms;

public static class Utils {
    public static Random random = new Random(1);

    /// <summary>
    /// Calculates Random Point in Radius
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 RandomPointInRadiusXZ (float3 position, float radius, float minRadius=0) {
        float randomDistance = random.NextFloat() * radius + minRadius;
        float clampedRandomDistance = math.clamp(randomDistance,0f, radius);
        float3 randomDirection = random.NextFloat3Direction();
        // restrict the randpoint on the x- and z-axis
        randomDirection.y = 0;
        float3 randomPosition = position + randomDirection * clampedRandomDistance;
        return randomPosition;
    }

    /// <summary>
    /// Caculates Random Rotation around y-axis
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternion GetRandomYRotation() {
        // Generate a random angle in radians between 0 and 2*PI
        float randomAngle = random.NextFloat(0f, math.PI * 2f);

        // Create a quaternion representing rotation around Y axis by randomAngle
        return quaternion.AxisAngle(math.up(), randomAngle);
    }

}
