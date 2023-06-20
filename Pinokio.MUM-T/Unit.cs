using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinokio.MUM_T
{
    internal class Unit
    {
        //기종, 타격가능(지/공), 작전 범위, 연료, 무장, 라우팅, 가능 임무, 현재 위치
        private string _type;
        private List<string> _attackRange;
        private string _operationRange;
        private float _fuel;
        private float _fuelSpeed;
        private float _fuelRate;
        private Dictionary<string, Dictionary<string,int>> _arm
    }
}
