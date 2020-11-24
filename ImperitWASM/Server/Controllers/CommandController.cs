using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion.Commands;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using ImperitWASM.Client.Data;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommandController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly ISessionService login;
		readonly IConfig cfg;
		readonly IContextService ctx;
		readonly IEndOfTurn end;
		readonly IActive active;
		public CommandController(IPlayersProvinces pap, ISessionService login, IConfig cfg, IEndOfTurn end, IActive active, IContextService ctx)
		{
			this.pap = pap;
			this.login = login;
			this.cfg = cfg;
			this.end = end;
			this.active = active;
			this.ctx = ctx;
		}

		bool Validate(int playerId, int gameId, string loginId) => login.IsValid(playerId, gameId, loginId) && playerId == active[gameId];
		[HttpPost("GiveUp")]
		public async Task GiveUp([FromBody] Session player)
		{
			if (login.IsValid(player.U, player.G, player.I))
			{
				_ = await pap.AddAsync(player.G, new GiveUp(pap.Player(player.G, player.U)));
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
		public async Task<Client.Data.MoveErrors> Move([FromBody] Client.Data.MoveCmd m)
		{
			if (!Validate(m.LoggedIn, m.Game, m.LoginId))
			{
				return MoveErrors.NotPlaying;
			}
			var p_p = pap[m.Game];
			var (from, to) = (p_p.Province(m.From), p_p.Province(m.To));
			var move = new Move(p_p.Player(m.LoggedIn), from, to, new Soldiers(m.Counts.Select((count, i) => (from.Soldiers[i].Type, count))));
			return (await pap.AddAsync(m.Game, move)) switch
			{
				true => MoveErrors.Ok,
				false when !from.Soldiers.Contains(move.Soldiers) => Client.Data.MoveErrors.FewSoldiers,
				false when move.Soldiers.Capacity(p_p, from, to) < 0 => Client.Data.MoveErrors.LittleCapacity,
				_ => MoveErrors.Else
			};
		}
		[HttpPost("PurchaseInfo")]
		public PurchaseInfo PurchaseInfo([FromBody] CmdData purchase)
		{
			var (player, land, gameId) = purchase;
			var p_p = pap[gameId];
			return new Client.Data.PurchaseInfo(new Buy(p_p.Player(player), p_p.Province(land), 0).Allowed(p_p), p_p.Province(land).Name, (p_p.Province(land) as Land)?.Price ?? int.MaxValue, p_p.Player(player).Money);
		}
		[HttpPost("Purchase")]
		public void Purchase([FromBody] PurchaseCmd purchase)
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
		public RecruitInfo RecruitInfo([FromBody] CmdData p)
		{
			var p_p = pap[p.G];
			return new RecruitInfo(p_p.Province(p.A).Name, p_p.Province(p.A).Soldiers.ToString(), cfg.Settings.RecruitableTypes(p_p.Province(p.A)).Select(t => new Client.Data.SoldiersItem(t.Description, t.Price)).ToImmutableArray(), p_p.Player(p.B).Money);
		}
		[HttpPost("Recruit")]
		public async Task Recruit([FromBody] RecruitCmd r)
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
				await ctx.SaveAsync();
			}
		}
		[HttpPost("Donate")]
		public async Task<bool> Donate([FromBody] DonationCmd donation)
		{
			return login.IsValid(donation.LoggedIn, donation.Game, donation.LoginId) && await pap.AddAsync(donation.Game, new Donate(pap.Player(donation.Game, donation.LoggedIn), pap.Player(donation.Game, donation.Recipient), donation.Amount));
		}
	}
}