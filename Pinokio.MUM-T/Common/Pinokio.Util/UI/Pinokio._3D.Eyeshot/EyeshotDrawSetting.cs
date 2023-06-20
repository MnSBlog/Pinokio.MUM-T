using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using Pinokio.Geometry;

namespace Pinokio._3D.Eyeshot
{
    public class EyeshotDrawSetting : DrawSetting
    {
        private bool _drawByBlock;
        private string _blockName;

        public bool DrawByBlock { get => _drawByBlock; set => _drawByBlock = value; }
        public string BlockName { get => _blockName; }

        public EyeshotDrawSetting(bool drawByBlock, string blockName)
        {
            _drawByBlock = drawByBlock;
            _blockName = blockName;
        }
    }
}
