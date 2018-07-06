using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace VPS.Common.Services
{
	public class DefaultLoggingService : ILoggerService
	{
        private static ILog log = LogManager.GetLogger<DefaultLoggingService>();

		public void Debug(Action<FormatMessageHandler> action)
		{
			log.Debug(action);
		}
	}
}
