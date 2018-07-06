namespace VPS.Common.Services
{
	public class JobInfo
	{
		public JobInfo(string commandType, int unprocessedCount, int processedCount, int skippedCount, int failedCount)
		{
			CommandType = commandType;
			UnprocessedCount = unprocessedCount;
			ProcessedCount = processedCount;
			SkippedCount = skippedCount;
			FailedCount = failedCount;
		}

		public string CommandType { get; set; }
		public int UnprocessedCount { get; set; }
		public int ProcessedCount { get; set; }
		public int FailedCount { get; set; }
		public int SkippedCount { get; set; }
	}
}