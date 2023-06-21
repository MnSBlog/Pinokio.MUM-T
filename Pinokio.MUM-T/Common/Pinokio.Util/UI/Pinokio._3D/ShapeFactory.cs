using System;
using System.Collections.Generic;
using System.Text;

using Pinokio.Simulation;

namespace Pinokio._3D
{
    public class ShapeFactory
    {
        private Dictionary<string, DrawSetting> _drawSettings;
        
        public Dictionary<string, DrawSetting> DrawSettings { get => _drawSettings; }

        public ShapeFactory()
        {
            _drawSettings = new Dictionary<string, DrawSetting>();
        }

        public virtual void AddModelShapes(Dictionary<uint, SimModel> models)
        {
            
        }

        public void DefineDrawSettings()
        {
            
        }

        public void AddDrawSetting(string name, DrawSetting drawSetting)
        {
            if (!_drawSettings.ContainsKey(name))
            {
                _drawSettings.Add(name, drawSetting);
            }
        }
    }
}
