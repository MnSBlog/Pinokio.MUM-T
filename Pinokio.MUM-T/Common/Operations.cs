using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.MUM_T.Common
{
    internal class Operations
    {
        public int Mission; // 미션 종류 (2진): 000001(정찰) 000010(타격) 000100(섬멸) 001000(보호) 010000(방어) 100000(억제)
        public int MissionLevel; // 미션 레벨: 1(Main) 2(Sub)
        public double ExpectMissionTime; //예상 수행 시간
        public double ExpectStartTime; //예상 시작 시간
        public double StartTime; // 시작 시간
        public double TotalDistance;
        public int ThreatLevel; //1~5..1등급이 최고위협
        public double OperationScope; //작전 범위
        public List<Unit> Units;
        public Tuple<Location, Location> OperationLocations; //작전 수행 위치(시작, 끝)
    }

    internal class Mission
    {
        
    }

    internal class Task
    {
        public int Id = -1;
        public List<Unit> TeamUnits = new List<Unit>();     // 아군 유닛
        public List<Unit> TargetUnits = new List<Unit>();   // 목표 유닛
        public DateTime ElapsedTime;                        // 흘러간 시간
        public DateTime StartTime;                          // 시작 시간
        public DateTime DueDate;                            // 제한 시간
        public int Priority;                                // 우선순위
        public float Threat;                                // 태스크 자체 위협도
        public Tuple<float, float, float>? TargetLocation;  // 목표 위치
    }

    internal class Unit
    {
        public int Id = -1;
        public int Kind = 0;    // 대상: 001(지상) 010(해상) 100(공중)
        public List<Weapon> Weapons = new();    // 무장
        public float? Distance;  // 전투행동반경
        public float? Speed;    // 임의의 속도
        public float? Fuel;     // 연료
        public float? FuelSpeed; // 연료 소모 속도
        public bool Progress; // 작전 수행 여부
        public object? RoutingFn;   // 라우팅 펑션
        public Location? Location; // 내 현재위치
    }

    internal class Weapon
    {
        public int Id = -1; // 종류
        public string? Name; // 이름
        public string? Description; // 설명
        public int? Target; // 공격가능한 대상 (2진): 001(지상) 010(해상) 100(공중) 101(지상+공중) 111(지상+해상+공중)
        public float? Weight;   // 물리 계산용
        public float? Distance; // 유효 범위
    }

    internal class Location
    {
        public float latitude; // 위도
        public float longitude; // 경도
    }
}
