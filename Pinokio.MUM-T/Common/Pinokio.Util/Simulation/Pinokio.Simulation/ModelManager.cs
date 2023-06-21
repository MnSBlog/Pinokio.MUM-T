using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Pinokio.Simulation
{
    public class ModelManager
    {
        private uint _modelId;
        private Dictionary<uint, SimModel> _models;

        public Dictionary<uint, SimModel> Models { get => _models; }

        public ModelManager()
        {
            _modelId = 0;
            _models = new Dictionary<uint, SimModel>();
        }

        public uint GetNextModelId()
        {
            return ++_modelId;
        }

        public void AddSimModel(SimModel model)
        {
            _models.Add(model.Id, model);
        }

    }
}
