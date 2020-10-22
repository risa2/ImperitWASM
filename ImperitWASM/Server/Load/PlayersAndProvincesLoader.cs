using ImperitWASM.Shared.State;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading.Tasks;

namespace ImperitWASM.Server.Load
{
	public class PlayersAndProvincesLoader
	{
		readonly IFile active_file, provinces_file;
		readonly Settings settings;
		readonly JsonWriter<JsonPlayer, Player, Settings> player_loader;
		readonly Graph graph;
		readonly ImmutableArray<Shape> shapes;
		public PlayersAndProvincesLoader(IFile provinces, IFile graph_file, IFile shape_file, IFile players, IFile active, Settings set)
		{
			settings = set;
			player_loader = new JsonWriter<JsonPlayer, Player, Settings>(players, settings, JsonPlayer.From);
			shapes = new JsonLoader<JsonShape, Shape, bool>(shape_file, false).Load().ToImmutableArray();
			graph = new JsonLoader<JsonGraph, Graph, bool>(graph_file, false).LoadOne();
			(active_file, provinces_file) = (active, provinces);
		}
		public PlayersAndProvinces Load()
		{
			var players = player_loader.Load().ToImmutableArray();
			var province_loader = new JsonLoader<JsonProvince, Province, (Settings, IReadOnlyList<Player>, IReadOnlyList<Shape>)>(provinces_file, (settings, players, shapes));
			var provinces = new Provinces(province_loader.Load().ToImmutableArray(), graph);
			int active = int.Parse(active_file.Read(), CultureInfo.InvariantCulture);
			return new PlayersAndProvinces(players, provinces, active);
		}
		public async Task Save(PlayersAndProvinces saved)
		{
			await player_loader.Save(saved.Players);
			await active_file.Write(saved.ToString());
			var province_loader = new JsonWriter<JsonProvince, Province, (Settings, IReadOnlyList<Player>, IReadOnlyList<Shape>)>(provinces_file, (settings, saved.Players, shapes), JsonProvince.From);
			await province_loader.Save(saved);
		}
	}
}