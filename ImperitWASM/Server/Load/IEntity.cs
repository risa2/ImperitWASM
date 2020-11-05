namespace ImperitWASM.Server.Load
{
	public interface IEntity
	{
		int Id { get; set; }
	}
	public interface IEntity<TResult, TArg> : IEntity
	{
		TResult Convert(TArg arg);
	}
}
