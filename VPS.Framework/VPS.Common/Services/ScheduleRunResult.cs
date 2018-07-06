namespace VPS.Common.Services
{
	public class ScheduleRunResult
	{
		public ScheduleRunResult(int commandsProcessed, int skipped, int failed)
		{
			CommandsProcessed = commandsProcessed;
			CommandsSkipped = skipped;
			CommandsFailed = failed;
		}


		public int CommandsProcessed { get; protected set; }
		public int CommandsSkipped { get; set; }
		public int CommandsFailed { get; set; }
	}
}