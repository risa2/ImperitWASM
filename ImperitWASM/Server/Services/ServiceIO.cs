using ImperitWASM.Server.Load;

namespace ImperitWASM.Server.Services
{
	public interface IServiceIO
	{
		IFile Settings { get; }
		IFile Players { get; }
		IFile Provinces { get; }
		IFile Active { get; }
		IFile Graph { get; }
		IFile Shapes { get; }
		IFile Powers { get; }
		IFile Game { get; }
		IFile FormerPlayers { get; }
		IFile Sessions { get; }
	}
	public class ServiceIO : IServiceIO
	{
		public IFile Settings { get; }
		public IFile Players { get; }
		public IFile Provinces { get; }
		public IFile Active { get; }
		public IFile Graph { get; }
		public IFile Shapes { get; }
		public IFile Powers { get; }
		public IFile Game { get; }
		public IFile FormerPlayers { get; }
		public IFile Sessions { get; }
		public ServiceIO(IFile settings, IFile players, IFile provinces, IFile active, IFile graph, IFile shapes, IFile powers, IFile game, IFile formerPlayers, IFile sessions)
		{
			Settings = settings;
			Players = players;
			Provinces = provinces;
			Active = active;
			Graph = graph;
			Shapes = shapes;
			Powers = powers;
			Game = game;
			FormerPlayers = formerPlayers;
			Sessions = sessions;
		}
	}
}