using System;
using System.Collections.Generic;
using VPS.Common.Commands;

namespace VPS.Common.Services
{
	public interface ISchedulerService
	{
		void Enqueue(CommandBase command, DateTime? runAt = null);
		IEnumerable<JobInfo> GetJobStats();
		ScheduleRunResult Run(bool throwExceptions = false);
		void AddHandler<T>(ICommandHandler<T> commandHandler) where T : CommandBase;
	}
}
