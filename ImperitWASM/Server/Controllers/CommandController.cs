using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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
	public class CommandController : ControllerBase
	{
		readonly IProvinceLoader province_load;
		readonly ISessionLoader session;
		readonly Settings settings;
		readonly ICommandExecutor cmd;
		readonly IEndOfTurn eot;
		readonly IPlayerLoader player_load;
		public CommandController(IProvinceLoader province_load, ISessionLoader session, Settings settings, IEndOfTurn eot, IPlayerLoader player_load, ICommandExecutor cmd)
		{
			this.province_load = province_load;
			this.session = session;
			this.settings = settings;
			this.eot = eot;
			this.player_load = player_load;
			this.cmd = cmd;
		}

		bool Validate(int playerId, int gameId, string loginId) => session.IsValid(playerId, gameId, loginId) && playerId == active[gameId];
		[HttpPost("GiveUp")]
		public async Task GiveUp([FromBody] Session player)
		{
			if (session.IsValid(player.P, player.G, player.Key))
			{
				_ = await province_load.AddAsync(player.G, new GiveUp(province_load.Player(player.G, player.P)));
				if (active[player.G] == player.P)
				{
					_ = await eot.NextTurnAsync(player.G);
				}
			}
		}
		[HttpPost("MoveInfo")]
		public MoveInfo MoveInfo([FromBody] MoveData move) => province_load.GameExists(move.G) && province_load[move.G] is PlayersAndProvinces p_p && p_p.Province(move.F) is Province from && p_p.Province(move.T) is Province to ? new MoveInfo(from.CanAnyMove(p_p, to), !from.IsAllyOf(to), to.Inhabited, from.Name, to.Name, from.Soldiers.ToString(), to.Soldiers.ToString(), from.Soldiers.Select(reg => reg.Type.Description).ToImmutableArray()) : new MoveInfo(false, false, false, "", "", "", "", ImmutableArray<Description>.Empty);
		[HttpPost("Move")]
		public async Task<MoveErrors> Move([FromBody] MoveCmd m)
		{
			if (!Validate(m.P, m.Game, m.Key))
			{
				return MoveErrors.NotPlaying;
			}
			var p_p = province_load[m.Game];
			var (from, to) = (p_p.Province(m.From), p_p.Province(m.To));
			var move = new Move(p_p.Player(m.P), from, to, new Soldiers(m.Counts.Select((count, i) => new Regiment(from.Soldiers[i].Type, count))));
			return (await province_load.AddAsync(m.Game, move)) switch
			{
				true => MoveErrors.Ok,
				_ when !from.Has(move.Soldiers) => MoveErrors.FewSoldiers,
				_ when !move.HasEnoughCapacity(p_p) => MoveErrors.LittleCapacity,
				_ => MoveErrors.Else
			};
		}
		[HttpPost("PurchaseInfo")]
		public PurchaseInfo PurchaseInfo([FromBody] PurchaseData purchase) => province_load.GameExists(purchase.G) && province_load[purchase.G] is PlayersAndProvinces p_p && p_p.Province(purchase.L) is Land land && p_p.Player(purchase.P) is Player player ? new PurchaseInfo(new Buy(player, land).AllowedWithLoan(p_p), land.Name, land.Price, player.Money) : new PurchaseInfo(false, "", 0, 0);
		[HttpPost("Purchase")]
		public async Task Purchase([FromBody] PurchaseCmd purchase)
		{
			if (Validate(purchase.P, purchase.Game, purchase.Key))
			{
				var p_p = province_load[purchase.Game];
				if (p_p.Province(purchase.Land) is Land Land && p_p.Player(purchase.P) is Player Player)
				{
					p_p = Land.Price > Player.Money ? p_p.Add(new Borrow(Player, Land.Price - Player.Money)) : p_p;
					province_load[purchase.Game] = p_p.Add(new Buy(Player, Land));
				}
				await cmd.SaveAsync();
			}
		}
		[HttpPost("RecruitInfo")]
		public RecruitInfo RecruitInfo([FromBody] RecruitData p) => province_load.Province(p.G, p.W) is Province province ? new RecruitInfo(province.Name, province.Soldiers.ToString(), settings.RecruitableTypes(province).Select(t => new SoldiersItem(t.Description, t.Price)).ToImmutableArray(), province_load.Player(p.G, p.P).Money, province is Land L ? L.Instability : new Ratio()) : new RecruitInfo("", "", ImmutableArray<SoldiersItem>.Empty, 0, new Ratio());
		[HttpPost("Recruit")]
		public async Task Recruit([FromBody] RecruitCmd r)
		{
			if (Validate(r.P, r.Game, r.Key))
			{
				var p_p = province_load[r.Game];
				var types = settings.RecruitableTypes(p_p.Province(r.Province)).ToImmutableArray();
				var soldiers = new Soldiers(r.Counts.Select((count, i) => new Regiment(types[i], count)));

				if (soldiers.Price > p_p.Player(r.P).Money)
				{
					p_p = p_p.Add(new Borrow(p_p.Player(r.P), soldiers.Price - p_p.Player(r.P).Money));
				}
				province_load[r.Game] = p_p.Add(new Recruit(p_p.Player(r.P), p_p.Province(r.Province), soldiers));
				await cmd.SaveAsync();
			}
		}
		[HttpPost("Donate")]
		public async Task<bool> Donate([FromBody] DonationCmd donation) => session.IsValid(donation.P, donation.Game, donation.Key) && await province_load.AddAsync(donation.Game, new Donate(province_load.Player(donation.Game, donation.P), province_load.Player(donation.Game, donation.Recipient), donation.Amount));
	}
}