using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Cmd;
using ImperitWASM.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommandController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly ISessionService login;
		readonly IConfig cfg;
		readonly IEndOfTurn end;
		readonly IActive active;
		public CommandController(IPlayersProvinces pap, ISessionService login, IConfig cfg, IEndOfTurn end, IActive active)
		{
			this.pap = pap;
			this.login = login;
			this.cfg = cfg;
			this.end = end;
			this.active = active;
		}
		bool Validate(int playerId, int gameId, string loginId) => login.IsValid(playerId, gameId, loginId) && playerId == active[gameId];
		[HttpPost("GiveUp")]
		public async Task GiveUp([FromBody] Client.Server.Session player)
		{
			if (login.IsValid(player.U, player.G, player.I))
			{
				_ = pap.Do(player.G, new GiveUp(pap.Player(player.G, player.U)));
				if (active[player.G] == player.U)
				{
					_ = await end.NextTurnAsync(player.G);
				}
			}
		}
		[HttpPost("MoveInfo")]
		public Client.Server.MoveInfo MoveInfo([FromBody] Client.Server.CmdData move)
		{
			var (from, to, game) = move;
			var p_p = pap[game];
			var possible = p_p.Province(from).Soldiers.Any(reg => reg.Type.CanMoveAlone(p_p, p_p.Province(from), p_p.Province(to)));
			var types = p_p.Province(from).Soldiers.Select(reg => reg.Type.Description).ToArray();
			return new Client.Server.MoveInfo(possible, !p_p.Province(from).IsAllyOf(p_p.Province(to)), p_p.Province(to).Occupied, p_p.Province(from).Name, p_p.Province(to).Name, p_p.Province(from).Soldiers.ToString(), p_p.Province(to).Soldiers.ToString(), types);
		}
		[HttpPost("Move")]
		public Client.Pages.Move.Errors Move([FromBody] Client.Server.MoveCmd m)
		{
			if (!Validate(m.LoggedIn, m.Game, m.LoginId))
			{
				return Client.Pages.Move.Errors.NotPlaying;
			}
			var p_p = pap[m.Game];
			var (from, to) = (p_p.Province(m.From), p_p.Province(m.To));
			var move = new Move(p_p.Player(m.LoggedIn), from, to, new Soldiers(m.Counts.Select((count, i) => new Regiment(from.Soldiers[i].Type, count)).ToImmutableArray()));
			return pap.Do(m.Game, move) switch
			{
				true => Client.Pages.Move.Errors.Ok,
				false when !from.Soldiers.Contains(move.Soldiers) => Client.Pages.Move.Errors.FewSoldiers,
				false when move.Soldiers.Capacity(p_p, from, to) < 0 => Client.Pages.Move.Errors.LittleCapacity,
				_ => Client.Pages.Move.Errors.Else
			};
		}
		[HttpPost("PurchaseInfo")]
		public Client.Server.PurchaseInfo PurchaseInfo([FromBody] Client.Server.CmdData purchase)
		{
			var (player, land, gameId) = purchase;
			var p_p = pap[gameId];
			return new Client.Server.PurchaseInfo(new Buy(p_p.Player(player), p_p.Province(land), 0).Allowed(cfg.Settings, p_p), p_p.Province(land).Name, (p_p.Province(land) as Land)?.Price ?? int.MaxValue, p_p.Player(player).Money);
		}
		[HttpPost("Purchase")]
		public void Purchase([FromBody] Client.Server.PurchaseCmd purchase)
		{
			var p_p = pap[purchase.Game];
			if (Validate(purchase.LoggedIn, purchase.Game, purchase.LoginId) && p_p.Province(purchase.Land) is Land Land)
			{
				if (Land.Price > p_p.Player(purchase.LoggedIn).Money)
				{
					(p_p, _) = p_p.Do(new Borrow(p_p.Player(purchase.LoggedIn), Land.Price - p_p.Player(purchase.LoggedIn).Money));
				}
				(pap[purchase.Game], _) = p_p.Do(new Buy(p_p.Player(purchase.LoggedIn), p_p.Province(purchase.Land), Land.Price));
			}
		}
		[HttpPost("RecruitInfo")]
		public Client.Server.RecruitInfo RecruitInfo([FromBody] Client.Server.CmdData p)
		{
			var p_p = pap[p.G];
			return new Client.Server.RecruitInfo(p_p.Province(p.A).Name, p_p.Province(p.A).Soldiers.ToString(), cfg.Settings.RecruitableTypes(p_p.Province(p.A)).Select(t => new Client.Server.SoldiersItem(t.Description, t.Price)).ToArray(), p_p.Player(p.B).Money);
		}
		[HttpPost("Recruit")]
		public void Recruit([FromBody] Client.Server.RecruitCmd r)
		{
			if (Validate(r.LoggedIn, r.Game, r.LoginId))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => new Regiment(cfg.Settings.SoldierTypes[i], count)).ToImmutableArray());
				var p_p = pap[r.Game];
				if (soldiers.Price > p_p.Player(r.LoggedIn).Money)
				{
					(p_p, _) = p_p.Do(new Borrow(p_p.Player(r.LoggedIn), soldiers.Price - p_p.Player(r.LoggedIn).Money));
				}
				(pap[r.Game], _) = p_p.Do(new Recruit(p_p.Player(r.LoggedIn), p_p.Province(r.Province), soldiers));
			}
		}
		[HttpPost("Donate")]
		public bool Donate([FromBody] Client.Server.DonationCmd donation)
		{
			return login.IsValid(donation.LoggedIn, donation.Game, donation.LoginId) && pap.Do(donation.Game, new Donate(pap.Player(donation.Game, donation.LoggedIn), pap.Player(donation.Game, donation.Recipient), donation.Amount));
		}
	}
}