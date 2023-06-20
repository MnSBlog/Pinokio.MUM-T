using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.MUM_T.Common
{
    internal class Operations
    {
    }

    internal class Unit
    {
        public int Id = 0;
        public int Kind = 0;    // 대상: 001(지상) 010(해상) 100(공중)
        public int Mission; // 미션 종류 (2진): 000001(정찰) 000010(타격) 000100(섬멸) 001000(보호) 010000(방어) 100000(억제)
        public List<Weapon> Weapons = new();    // 무장
        public float? Distance;  // 작전 범위
        public float? Speed;    // 임의의 속도
        public float? Fuel;     // 연료
        public float? FuelSpeed; // 연료 소모 속도
        public object? RoutingFn;   // 라우팅 펑션
        public Tuple<float, float, float>? Location; // 내 현재위치
    }

    internal class Weapon
    {
        public int Id;
        public string? Name;
        public string? Description;
        public int? Target; // 공격가능한 대상 (2진): 001(지상) 010(해상) 100(공중) 101(지상+공중) 111(지상+해상+공중)
        public float? Weight;   // 물리 계산용
        public float? Distance; // 유효 범위
    }
}
