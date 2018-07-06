namespace VPS.Common.Commands
{
	public interface ICommandHandler<in T> where T : CommandBase
	{
		void Execute(T command);
	}
}