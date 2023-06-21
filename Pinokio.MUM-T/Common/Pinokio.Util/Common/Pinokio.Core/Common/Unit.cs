using System;
using System.Collections.Generic;

namespace Pinokio.Core
{
    public class Unit
    {
        #region Static
        private static Unit _mm = new Unit(TYPE.MM);
        private static Unit _cm = new Unit(TYPE.CM);
        private static Unit _m = new Unit(TYPE.M);
        public static Unit MM { get { return _mm; } }
        public static Unit CM { get { return _cm; } }
        public static Unit M { get { return _m; } }
        #endregion

        public enum TYPE
        {
            NONE,
            MM, CM, M,
        }

        private TYPE _type;

        /// <summary>
        /// Model들의 위치 단위를 보정하기 위한 Parameter
        /// Model들은 기본적으로 mm를 기본 단위로 사용한다
        /// ex) cm --> mm / m --> mm
        /// </summary>
        private static Dictionary<TYPE, int> _parameter = new Dictionary<TYPE, int>()
        {
            { TYPE.MM, 1 },
            { TYPE.CM, 10 },
            { TYPE.M, 1000 },
        };
        public TYPE Type { get { return _type; } }

        public int Constant { get { return _parameter[_type]; } }

        public static int Constance_MM
        {
            get => _parameter[TYPE.MM];
        }
        public static int Constance_CM
        {
            get => _parameter[TYPE.CM];
        }
        public static int Constance_M
        {
            get => _parameter[TYPE.M];
        }

        public Unit(TYPE type)
        {
            _type = type;
        }

        public void SetCurrentUnit(string unit)
        {
            switch (unit)
            {
                case "mm":
                case "MM":
                    _type = TYPE.MM;
                    break;
                case "cm":
                case "CM":
                    _type = TYPE.CM;
                    break;
                case "m":
                case "M":
                    _type = TYPE.M;
                    break;
                default:
                    throw new Exception("Wrong Unit Type");
            }
        }

        public override string ToString()
        {
            switch (_type)
            {
                case TYPE.MM: return "mm";
                case TYPE.M: return "m";
                case TYPE.CM: return "cm";
                default: return "";
            }
        }
    }
}