using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Core
{
    public class Loader
    {
        public Action<string> OnLoadingHandler = null;
        public Action<string> OnLoadedHandler = null;

        public void LoadStart()
        {
            //try
            //{
            //    this.Load();
            //}
            //catch (Exception e)
            //{
            //    LogHandler.AddLog(LogLevel.Error, e.ToString());
            //}
            this.Load();
        }

        protected virtual void Load()
        {
        }

        protected void OnLoading(string msg)
        {
            if (OnLoadingHandler != null)
            {
                OnLoadingHandler("Loading... " + msg);
            }
        }

        protected void OnLoaded(string msg)
        {
            if (OnLoadedHandler != null)
            {
                OnLoadedHandler("Load Finished " + msg);
            }
        }
    }
}
