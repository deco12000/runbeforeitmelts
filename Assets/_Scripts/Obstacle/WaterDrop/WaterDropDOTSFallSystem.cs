
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
[BurstCompile]
public partial struct WaterDropFallSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // StoppedTag가 없는 빗방울 엔티티만 낙하 로직 적용
        // Job 정의 시 ApplyGravityJob에 WaterDropFallAspect를 사용합니다.
        var job = new ApplyGravityJob
        {
            DeltaTime = deltaTime
        };

        // ScheduleParallel()은 IJobEntity를 구현한 Job 구조체와 함께 사용되며,
        // Job 구조체가 Execute 메서드의 파라미터로 Aspect를 가질 때,
        // 해당 Aspect에 포함된 모든 컴포넌트 조합을 가진 엔티티들을 자동으로 찾아 Job을 실행합니다.
        job.ScheduleParallel();
    }

    public void OnCreate(ref SystemState state) {}
    public void OnDestroy(ref SystemState state) {}
}

[BurstCompile]
// IJobEntity는 이제 WaterDropFallAspect를 Execute 파라미터로 받습니다.
// 이 Job은 WaterDropFallAspect에 정의된 모든 컴포넌트(Translation, Velocity, Gravity)를 가진 엔티티에 대해 실행됩니다.
// StoppedTag를 제외하는 것은 시스템 레벨의 쿼리 필터링이 필요합니다.
public partial struct ApplyGravityJob : IJobEntity
{
    public float DeltaTime;

    // Execute 메서드 파라미터가 Aspect 타입으로 변경되었습니다.
    // Aspect를 통해 컴포넌트 데이터에 접근합니다.
    void Execute(WaterDropFallAspect waterDrop) // ref 키워드는 필요 없음 (Aspect 자체가 값 타입 구조체)
    {
        // Aspect의 RefRW/RefRO를 사용하여 컴포넌트 값에 접근하고 수정합니다.
        waterDrop.Velocity.ValueRW.Value += waterDrop.Gravity.ValueRO.Value * DeltaTime;
        waterDrop.LocalTransform.ValueRW.Position += waterDrop.Velocity.ValueRO.Value * DeltaTime;
    }
}