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
		readonly IPlayerLoader player_load;
		readonly IDatabase db;
		public CommandController(IProvinceLoader province_load, ISessionLoader session, Settings settings, IPlayerLoader player_load, ICommandExecutor cmd, IDatabase db)
		{
			this.province_load = province_load;
			this.session = session;
			this.settings = settings;
			this.player_load = player_load;
			this.cmd = cmd;
			this.db = db;
		}

		[HttpPost("GiveUp")] public void GiveUp([FromBody] Session player)
		{
			if (session.IsValid(player.P, player.G, player.Key))
			{
				_ = cmd.Perform(player.G, player.P, new GiveUp(), false);
			}
		}
		[HttpPost("MoveInfo")] public MoveInfo MoveInfo([FromBody] MoveData move)
		{
			var prov = province_load[move.G];
			return new MoveInfo(prov[move.F].CanAnyMove(prov, prov[move.T]), prov[move.F].Name, prov[move.T].Name, prov[move.F].Soldiers.ToString(), prov[move.T].Soldiers.ToString(), settings.SoldierTypes.Select(type => prov[move.F].Soldiers.CountOf(type)).ToImmutableArray());
		}
		[HttpPost("Move")] public MoveErrors Move([FromBody] MoveCmd m)
		{
			if (!session.IsValid(m.P, m.Game, m.Key))
			{
				return MoveErrors.NotPlaying;
			}
			return db.Transaction(true, () =>
			{
				var players = player_load[m.Game];
				var provinces = province_load[m.Game];
				var move = new Move(provinces[m.From], provinces[m.To], new Soldiers(m.Counts.Select((count, i) => new Regiment(settings.SoldierTypes[i], count))));
				
				return cmd.Perform(m.Game, m.P, move, players, provinces).Item1 switch
				{
					true => MoveErrors.Ok,
					_ when !provinces[m.From].Has(move.Soldiers) => MoveErrors.FewSoldiers,
					_ when !move.HasEnoughCapacity(provinces) => MoveErrors.LittleCapacity,
					_ => MoveErrors.Else
				};
			});
		}
		[HttpPost("PurchaseInfo")] public PurchaseInfo PurchaseInfo([FromBody] PurchaseData purchase)
		{
			var player = player_load[purchase.G, purchase.P];
			var provinces = province_load[purchase.G];
			return provinces[purchase.L].Mainland ? new PurchaseInfo(new Buy(provinces[purchase.L]).Allowed(player, provinces), provinces[purchase.L].Name, provinces[purchase.L].Price, player.Money) : new PurchaseInfo(false, "", 0, 0);
		}
		[HttpPost("Purchase")] public void Purchase([FromBody] PurchaseCmd p)
		{
			if (session.IsValid(p.P, p.Game, p.Key))
			{
				_ = cmd.Perform(p.Game, p.P, new Buy(province_load[p.Game, p.Land]), false);
			}
		}

		[HttpPost("RecruitInfo")] public RecruitInfo RecruitInfo([FromBody] RecruitData p)
		{
			var player = player_load[p.G, p.P];
			var province = province_load[player.GameId, p.W];
			return new RecruitInfo(province.Name, province.Soldiers.ToString(), settings.SoldierTypes.Select(type => type.IsRecruitable(province.Region)).ToImmutableArray(), player.Money, province.Instability);
		}
		[HttpPost("Recruit")] public void Recruit([FromBody] RecruitCmd r)
		{
			if (session.IsValid(r.P, r.Game, r.Key))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => new Regiment(settings.SoldierTypes[i], count)));
				_ = cmd.Perform(r.Game, r.P, new Recruit(province_load[r.Game, r.Province], soldiers), false);
			}
		}
		[HttpPost("Donate")] public bool Donate([FromBody] DonationCmd d)
		{
			return session.IsValid(d.P, d.Game, d.Key) && cmd.Perform(d.Game, d.P, new Donate(player_load[d.Game, d.Recipient], d.Amount), false).Item1;
		}
		[HttpPost("NextTurn")] public bool NextTurn([FromBody] Session ses)
		{
			return session.IsValid(ses.P, ses.G, ses.Key) && cmd.Perform(ses.G, ses.P, new NextTurn(), false).Item4 is { Current: Game.State.Finished };
		}
	}
}