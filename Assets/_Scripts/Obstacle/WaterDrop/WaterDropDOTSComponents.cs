using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
public struct WaterDropVelocity : IComponentData
{
    public float3 Value;
}
public struct WaterDropGravity : IComponentData
{
    public float3 Value;
}

// GroundTag 대신 WaterDropGroundLayer를 사용하신다면 이 컴포넌트 정의는 유지합니다.
public struct WaterDropGroundLayer : IComponentData
{
    
}

// 빗방방울의 낙하 관련 컴포넌트들을 묶는 Aspect
public readonly partial struct WaterDropFallAspect : IAspect
{
    public readonly Entity Self;
    // <-- Unity.Transforms.Translation은 Unity.Transforms.LocalTransform 으로 통합되었음
    public readonly RefRW<LocalTransform> LocalTransform;
    // Velocity 컴포넌트 (읽기/쓰기 필요) - 사용자 정의 Velocity 사용
    public readonly RefRW<WaterDropVelocity> Velocity;
    // Gravity 컴포넌트 (읽기 전용) - 사용자 정의 Gravity 사용
    public readonly RefRO<WaterDropGravity> Gravity;
    // StoppedTag 컴포넌트도 여기에 RefRO 또는 RefRW로 추가하여
    // Aspect를 통해 Stopped 상태를 확인할 수도 있습니다. (선택 사항)
    // public readonly RefRO<StoppedTag> Stopped;
}

// StoppedTag 컴포넌트 정의가 없다면 추가합니다.
public struct StoppedTag : IComponentData {}