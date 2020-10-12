namespace ImperitWASM.Shared.Conversion
{
	public interface IEntity<TResult, TArg>
	{
		TResult Convert(int i, TArg arg);
	}
}
