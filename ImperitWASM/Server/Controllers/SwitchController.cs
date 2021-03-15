using System.Linq;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SwitchController : ControllerBase
	{
		readonly IProvinceLoader province_load;
		readonly IPlayerLoader player_load;
		readonly Settings settings;
		public SwitchController(IProvinceLoader province_load, IPlayerLoader player_load, Settings settings)
		{
			this.province_load = province_load;
			this.player_load = player_load;
			this.settings = settings;
		}

		bool IsPossible(Provinces provinces, Player player, Switch s) => s.From is int from && s.To is int to && s.View switch
		{
			View.Recruit => settings.RecruitableIn(to).Any(),
			View.Move => provinces[from].CanAnyMove(provinces, provinces[to]),
			View.Purchase => provinces[to].Mainland && new Buy(provinces[to]).Allowed(player, provinces),
			_ => false
		};
		Switch IfPossible(Provinces provinces, Player player, Switch s) => IsPossible(provinces, player, s) ? s : new Switch(s.Select, View.Map, null, null);
		static Switch ClickedResult(Provinces provinces, Player player, Click c) => c.From switch
		{
			int start => new Switch(null, start == c.Clicked ? View.Recruit : View.Move, start, c.Clicked),
			_ when provinces[c.Clicked].IsAllyOf(player.Id) => new Switch(c.Clicked, View.Map, null, null),
			_ when provinces[c.Clicked] is { Mainland: true, Inhabited: false } => new Switch(null, View.Purchase, c.Clicked, c.Clicked),
			_ => new Switch()
		};
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Click c)
		{
			var player = player_load[c.Game, c.Player];
			var provinces = province_load[player.GameId];
			return IfPossible(provinces, player, player.Active ? ClickedResult(provinces, player, c) : new Switch());
		}
	}
}
