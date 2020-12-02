using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
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
		readonly Settings settings;
		readonly IContextService ctx;
		readonly IEndOfTurn end;
		readonly IActive active;
		public CommandController(IPlayersProvinces pap, ISessionService login, Settings settings, IEndOfTurn end, IActive active, IContextService ctx)
		{
			this.pap = pap;
			this.login = login;
			this.settings = settings;
			this.end = end;
			this.active = active;
			this.ctx = ctx;
		}

		bool Validate(int playerId, int gameId, string loginId) => login.IsValid(playerId, gameId, loginId) && playerId == active[gameId];
		[HttpPost("GiveUp")]
		public async Task GiveUp([FromBody] Session player)
		{
			if (login.IsValid(player.P, player.G, player.Key))
			{
				_ = await pap.AddAsync(player.G, new GiveUp(pap.Player(player.G, player.P)));
				if (active[player.G] == player.P)
				{
					_ = await end.NextTurnAsync(player.G);
				}
			}
		}
		[HttpPost("MoveInfo")]
		public MoveInfo MoveInfo([FromBody] CmdData move) => pap.GameExists(move.G) && pap[move.G] is PlayersAndProvinces p_p && p_p.Province(move.A) is Province from && p_p.Province(move.B) is Province to ? new MoveInfo(from.CanAnyMove(p_p, from, to), !from.IsAllyOf(to), to.Occupied, from.Name, to.Name, from.Soldiers.ToString(), to.Soldiers.ToString(), from.Soldiers.Select(reg => reg.Type.Description).ToImmutableArray()) : new MoveInfo(false, false, false, "", "", "", "", ImmutableArray<Description>.Empty);
		[HttpPost("Move")]
		public async Task<MoveErrors> Move([FromBody] MoveCmd m)
		{
			if (!Validate(m.P, m.Game, m.Key))
			{
				return MoveErrors.NotPlaying;
			}
			var p_p = pap[m.Game];
			var (from, to) = (p_p.Province(m.From), p_p.Province(m.To));
			var move = new Move(p_p.Player(m.P), from, to, new Soldiers(m.Counts.Select((count, i) => new Regiment(from.Soldiers[i].Type, count))));
			return (await pap.AddAsync(m.Game, move)) switch
			{
				true => MoveErrors.Ok,
				false when !from.Has(move.Soldiers) => MoveErrors.FewSoldiers,
				false when move.HasEnoughCapacity(p_p, from, to) => MoveErrors.LittleCapacity,
				_ => MoveErrors.Else
			};
		}
		[HttpPost("PurchaseInfo")]
		public PurchaseInfo PurchaseInfo([FromBody] CmdData purchase) => pap.GameExists(purchase.G) && pap[purchase.G] is PlayersAndProvinces p_p && p_p.Province(purchase.B) is Land land && p_p.Player(purchase.B) is Player player ? new PurchaseInfo(new Buy(player, land, 0).Allowed(p_p), land.Name, land.Price, player.Money) : new PurchaseInfo(false, "", 0, 0);
		[HttpPost("Purchase")]
		public void Purchase([FromBody] PurchaseCmd purchase)
		{
			if (Validate(purchase.P, purchase.Game, purchase.Key))
			{
				var p_p = pap[purchase.Game];
				if (p_p.Province(purchase.Land) is Land Land && p_p.Player(purchase.P) is Player Player)
				{
					if (Land.Price > Player.Money)
					{
						(p_p, _) = p_p.Add(new Borrow(Player, Land.Price - Player.Money, settings));
					}
					(pap[purchase.Game], _) = p_p.Add(new Buy(Player, Land, Land.Price));
				}
			}
		}
		[HttpPost("RecruitInfo")]
		public RecruitInfo RecruitInfo([FromBody] CmdData p) => pap.Province(p.G, p.A) is Province province ? new RecruitInfo(province.Name, province.Soldiers.ToString(), settings.RecruitableTypes(province).Select(t => new SoldiersItem(t.Description, t.Price)).ToImmutableArray(), pap.Player(p.G, p.B).Money) : new RecruitInfo("", "", ImmutableArray<SoldiersItem>.Empty, 0);
		[HttpPost("Recruit")]
		public async Task Recruit([FromBody] RecruitCmd r)
		{
			if (Validate(r.P, r.Game, r.Key))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => new Regiment(settings.SoldierTypes[i], count)));
				var p_p = pap[r.Game];
				if (soldiers.Price > p_p.Player(r.P).Money)
				{
					(p_p, _) = p_p.Add(new Borrow(p_p.Player(r.P), soldiers.Price - p_p.Player(r.P).Money, settings));
				}
				(pap[r.Game], _) = p_p.Add(new Recruit(p_p.Player(r.P), p_p.Province(r.Province), soldiers));
				await ctx.SaveAsync();
			}
		}
		[HttpPost("Donate")]
		public async Task<bool> Donate([FromBody] DonationCmd donation) => login.IsValid(donation.P, donation.Game, donation.Key) && await pap.AddAsync(donation.Game, new Donate(pap.Player(donation.Game, donation.P), pap.Player(donation.Game, donation.Recipient), donation.Amount));
	}
}