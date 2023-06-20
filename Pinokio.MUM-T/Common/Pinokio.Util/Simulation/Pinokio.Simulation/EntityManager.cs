using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation
{
    public class EntityManager
    {
        private uint _lastEntityId;
        private Dictionary<uint, SimEntity> _entities;

        public Dictionary<uint, SimEntity> Entities { get => _entities; }

        public EntityManager()
        {
            _lastEntityId = 0;
            _entities = new Dictionary<uint, SimEntity>();
        }

        public uint GetNextEntityId()
        {
            return ++_lastEntityId;
        }

        public void AddEntity(SimEntity entity)
        {
            _entities.Add(entity.Id, entity);
        }
    }
}
