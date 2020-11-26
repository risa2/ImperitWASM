using System.Linq;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion.Commands;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using Mode = ImperitWASM.Client.Data.View;
using Switch = ImperitWASM.Client.Data.Switch;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SwitchController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly IConfig cfg;
		readonly IActive active;
		public SwitchController(IPlayersProvinces pap, IConfig cfg, IActive active)
		{
			this.pap = pap;
			this.cfg = cfg;
			this.active = active;
		}

		bool IsPossible(PlayersAndProvinces p_p, int player, Switch s) => s.F is int from && s.T is int to && s.M switch
		{
			Mode.Recruit => cfg.Settings.RecruitableTypes(p_p.Province(to)).Any(),
			Mode.Move => p_p.Province(from).Soldiers.Any(reg => reg.Type.CanMoveAlone(p_p, p_p.Province(from), p_p.Province(to))),
			Mode.Purchase => new Buy(p_p.Player(player), p_p.Province(to), 0).Allowed(p_p),
			_ => false
		};
		Switch IfPossible(PlayersAndProvinces p_p, int player, Switch s) => IsPossible(p_p, player, s) ? s : new Switch(s.S, Mode.Map, null, null);
		static Switch ClickedResult(PlayersAndProvinces p_p, int user, int? from, int clicked) => from switch
		{
			int start => new Switch(null, start == clicked ? Mode.Recruit : Mode.Move, start, clicked),
			null when p_p.Province(clicked).IsAllyOf(p_p.Player(user)) => new Switch(clicked, Mode.Map, null, null),
			null when p_p.Province(clicked) is Land L1 && !L1.Occupied => new Switch(null, Mode.Purchase, clicked, clicked),
			_ => new Switch(null, Mode.Map, null, null)
		};
		[HttpPost("Clicked")]
		public Switch Clicked([FromBody] Client.Data.Click c)
		{
			var (p_p, player) = (pap[c.G], active[c.G]);
			return IfPossible(p_p, player, c.U == player ? ClickedResult(p_p, c.U, c.F, c.C) : new Switch(null, Mode.Map, null, null));
		}
	}
}
