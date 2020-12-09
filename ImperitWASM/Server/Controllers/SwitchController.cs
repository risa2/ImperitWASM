using System.Linq;
using ImperitWASM.Client.Data;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SwitchController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly Settings settings;
		readonly IActive active;
		public SwitchController(IPlayersProvinces pap, Settings settings, IActive active)
		{
			this.pap = pap;
			this.settings = settings;
			this.active = active;
		}

		bool IsPossible(PlayersAndProvinces p_p, int player, Switch s) => s.From is int from && s.To is int to && s.View switch
		{
			View.Recruit => settings.RecruitableTypes(p_p.Province(to)).Any(),
			View.Move => p_p.Province(from).CanAnyMove(p_p, p_p.Province(to)),
			View.Purchase => new Buy(p_p.Player(player), p_p.Province(to), 0).Allowed(p_p),
			_ => false
		};
		Switch IfPossible(PlayersAndProvinces p_p, int player, Switch s) => IsPossible(p_p, player, s) ? s : new Switch(s.Select, View.Map, null, null);
		static Switch ClickedResult(PlayersAndProvinces p_p, Click c) => c.From switch
		{
			int start => new Switch(null, start == c.Clicked ? View.Recruit : View.Move, start, c.Clicked),
			_ when p_p.Province(c.Clicked).IsAllyOf(p_p.Player(c.Player)) => new Switch(c.Clicked, View.Map, null, null),
			_ when p_p.Province(c.Clicked) is Land L1 && !L1.Occupied => new Switch(null, View.Purchase, c.Clicked, c.Clicked),
			_ => new Switch(null, View.Map, null, null)
		};
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Click c)
		{
			var (p_p, player) = (pap[c.Game], active[c.Game]);
			return IfPossible(p_p, player, c.Player == player ? ClickedResult(p_p, c) : new Switch(null, View.Map, null, null));
		}
	}
}
