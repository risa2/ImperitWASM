namespace ImperitWASM.Shared.Conversion
{
	public interface IEntity<TResult, TArg>
	{
		TResult Convert(TArg arg);
	}
}
