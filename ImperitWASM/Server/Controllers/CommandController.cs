using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion.Commands;
using ImperitWASM.Shared.State;
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
		public async Task GiveUp([FromBody] Client.Data.Session player)
		{
			if (login.IsValid(player.U, player.G, player.I))
			{
				_ = pap.Add(player.G, new GiveUp(pap.Player(player.G, player.U)));
				if (active[player.G] == player.U)
				{
					_ = await end.NextTurnAsync(player.G);
				}
			}
		}
		[HttpPost("MoveInfo")]
		public Client.Data.MoveInfo MoveInfo([FromBody] Client.Data.CmdData move)
		{
			var (from, to, game) = move;
			var p_p = pap[game];
			bool possible = p_p.Province(from).SoldierTypes.Any(type => type.CanMoveAlone(p_p, p_p.Province(from), p_p.Province(to)));
			var types = p_p.Province(from).SoldierTypes.Select(type => type.Description).ToImmutableArray();
			return new Client.Data.MoveInfo(possible, !p_p.Province(from).IsAllyOf(p_p.Province(to)), p_p.Province(to).Occupied, p_p.Province(from).Name, p_p.Province(to).Name, p_p.Province(from).Soldiers.ToString(), p_p.Province(to).Soldiers.ToString(), types);
		}
		[HttpPost("Move")]
		public Client.Pages.Move.Errors Move([FromBody] Client.Data.MoveCmd m)
		{
			if (!Validate(m.LoggedIn, m.Game, m.LoginId))
			{
				return Client.Pages.Move.Errors.NotPlaying;
			}
			var p_p = pap[m.Game];
			var (from, to) = (p_p.Province(m.From), p_p.Province(m.To));
			var move = new Move(p_p.Player(m.LoggedIn), from, to, new Soldiers(m.Counts.Select((count, i) => (from.Soldiers[i].Type, count))));
			return pap.Add(m.Game, move) switch
			{
				true => Client.Pages.Move.Errors.Ok,
				false when !from.Soldiers.Contains(move.Soldiers) => Client.Pages.Move.Errors.FewSoldiers,
				false when move.Soldiers.Capacity(p_p, from, to) < 0 => Client.Pages.Move.Errors.LittleCapacity,
				_ => Client.Pages.Move.Errors.Else
			};
		}
		[HttpPost("PurchaseInfo")]
		public Client.Data.PurchaseInfo PurchaseInfo([FromBody] Client.Data.CmdData purchase)
		{
			var (player, land, gameId) = purchase;
			var p_p = pap[gameId];
			return new Client.Data.PurchaseInfo(new Buy(p_p.Player(player), p_p.Province(land), 0).Allowed(p_p), p_p.Province(land).Name, (p_p.Province(land) as Land)?.Price ?? int.MaxValue, p_p.Player(player).Money);
		}
		[HttpPost("Purchase")]
		public void Purchase([FromBody] Client.Data.PurchaseCmd purchase)
		{
			var p_p = pap[purchase.Game];
			if (Validate(purchase.LoggedIn, purchase.Game, purchase.LoginId) && p_p.Province(purchase.Land) is Land Land)
			{
				if (Land.Price > p_p.Player(purchase.LoggedIn).Money)
				{
					(p_p, _) = p_p.Add(new Borrow(p_p.Player(purchase.LoggedIn), Land.Price - p_p.Player(purchase.LoggedIn).Money, cfg.Settings));
				}
				(pap[purchase.Game], _) = p_p.Add(new Buy(p_p.Player(purchase.LoggedIn), p_p.Province(purchase.Land), Land.Price));
			}
		}
		[HttpPost("RecruitInfo")]
		public Client.Data.RecruitInfo RecruitInfo([FromBody] Client.Data.CmdData p)
		{
			var p_p = pap[p.G];
			return new Client.Data.RecruitInfo(p_p.Province(p.A).Name, p_p.Province(p.A).Soldiers.ToString(), cfg.Settings.RecruitableTypes(p_p.Province(p.A)).Select(t => new Client.Data.SoldiersItem(t.Description, t.Price)).ToImmutableArray(), p_p.Player(p.B).Money);
		}
		[HttpPost("Recruit")]
		public void Recruit([FromBody] Client.Data.RecruitCmd r)
		{
			if (Validate(r.LoggedIn, r.Game, r.LoginId))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => (cfg.Settings.SoldierTypes[i], count)));
				var p_p = pap[r.Game];
				if (soldiers.Price > p_p.Player(r.LoggedIn).Money)
				{
					(p_p, _) = p_p.Add(new Borrow(p_p.Player(r.LoggedIn), soldiers.Price - p_p.Player(r.LoggedIn).Money, cfg.Settings));
				}
				(pap[r.Game], _) = p_p.Add(new Recruit(p_p.Player(r.LoggedIn), p_p.Province(r.Province), soldiers));
			}
		}
		[HttpPost("Donate")]
		public bool Donate([FromBody] Client.Data.DonationCmd donation)
		{
			return login.IsValid(donation.LoggedIn, donation.Game, donation.LoginId) && pap.Add(donation.Game, new Donate(pap.Player(donation.Game, donation.LoggedIn), pap.Player(donation.Game, donation.Recipient), donation.Amount));
		}
	}
}