using System;
using System.Text;

using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using log4net.Layout;

using Pinokio.Core;

namespace Pinokio.Log4Net
{
    public class Logger
    {
        public delegate void PrintLogHandler(string message);
        public event PrintLogHandler PrintInfoHandle = null;
        public event PrintLogHandler PrintDebugHandle = null;
        public event PrintLogHandler PrintWarnHandle = null;
        public event PrintLogHandler PrintErrorHandle = null;

        private ILog _logger = null;

        public Logger()
        { }

        public Logger(string name, string path = "log\\")
        {
            _logger = GenerateNewLogger(name, path);
        }

        public void SetLogger(ILog logger)
        {
            _logger = logger;
        }

        public static ILog GenerateNewLogger(string name, string path = "log\\")
        {
            var loggerName = name + DateTime.Now.ToString();
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Configured = true;

            RollingFileAppender rollingAppender = new RollingFileAppender();
            rollingAppender.Name = name;
            rollingAppender.File = path;
            rollingAppender.Encoding = Encoding.UTF8;
            rollingAppender.AppendToFile = false;

            rollingAppender.DatePattern = "_yyyyMMdd'.log'";
            rollingAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            rollingAppender.LockingModel = new log4net.Appender.RollingFileAppender.MinimalLock();
            rollingAppender.StaticLogFileName = false;
            rollingAppender.MaxSizeRollBackups = 100;
            rollingAppender.MaximumFileSize = "5MB";

            PatternLayout layout = new log4net.Layout.PatternLayout("%d %-5p - %m%n");
            rollingAppender.Layout = layout;
            rollingAppender.ActivateOptions();

            var repository = LogManager.CreateRepository(name);
            BasicConfigurator.Configure(repository, rollingAppender);
            ILog logger = LogManager.GetLogger(name, loggerName);
            return logger;
        }

        private static string GetPrefix(LogLevel level)
        {
            return $"{"[" + level.ToString() + "]",-6} ({DateTime.Now.ToString("hh:mm:ss.ff")}) ";
        }

        public void Info(string msg)
        {
            if (_logger != null)
            {
                _logger.Info(msg);
            }

            if (PrintInfoHandle != null)
            {
                PrintInfoHandle(GetPrefix(LogLevel.Info) + msg);
            }
        }

        public void Debug(string msg)
        {
            if (_logger != null)
            {
                _logger.Debug(msg);
            }

            if (PrintDebugHandle != null)
            {
                PrintDebugHandle(GetPrefix(LogLevel.Debug) + msg);
            }

        }

        public void Warn(string msg)
        {
            if (_logger != null)
            {
                _logger.Warn(msg);
            }

            if (PrintWarnHandle != null)
            {
                PrintWarnHandle(GetPrefix(LogLevel.Warn) + msg);
            }
        }

        public void Error(string msg)
        {
            if (_logger != null)
            {
                _logger.Warn(msg);
            }

            if (PrintErrorHandle != null)
            {
                PrintErrorHandle(GetPrefix(LogLevel.Error) + msg);
            }
        }
    }
}
