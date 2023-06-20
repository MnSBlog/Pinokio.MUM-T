using System;

using Pinokio.Core;

namespace Pinokio.Map
{
    public class MapLoader : Loader
    {

        protected PinokioMap Map;
        public MapLoader(PinokioMap map)
        {
            Map = map;
        }

        protected override void Load()
        {
            if (!LoadNodeData()) throw new Exception("Fail To Load Node Information");

            if (!LoadLinkData()) throw new Exception("Fail To Load Link Information");

            if (!LoadPortData()) throw new Exception("Fail To Load Port Information");

            if (!LoadProcessData()) throw new Exception("Fail To Load Process Information");

            if (!LoadVehicleTypeData()) throw new Exception("Fail To Load Vehicle Type Information");

            if (!LoadAGVData()) throw new Exception("Fail To Load AGV Information");

            if (!LoadOHTData()) throw new Exception("Fail to Load OHT Information");

            if (!LoadBufferData()) throw new Exception("Fail to Load Buffer Information");
        }

        #region [Load Methods]
        protected virtual bool LoadNodeData()
        {
            return true;
        }

        protected virtual bool LoadLinkData()
        {
            return true;
        }

        protected virtual bool LoadPortData()
        {
            return true;
        }

        protected virtual bool LoadProcessData()
        {
            return true;
        }

        protected virtual bool LoadVehicleTypeData()
        {
            return true;
        }

        protected virtual bool LoadAGVData()
        {
            return true;
        }

        protected virtual bool LoadOHTData()
        {
            return true;
        }

        protected virtual bool LoadBufferData()
        {
            return true;
        }
        #endregion [Load Methods]
    }
}