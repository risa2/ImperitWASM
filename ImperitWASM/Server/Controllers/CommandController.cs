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
		readonly ILoginService login;
		readonly ISettingsLoader sl;
		readonly IEndOfTurn end;
		public CommandController(IPlayersProvinces pap, ILoginService login, ISettingsLoader sl, IEndOfTurn end)
		{
			this.pap = pap;
			this.login = login;
			this.sl = sl;
			this.end = end;
		}
		bool Validate(int loggedIn, string loginId) => login.Get(loggedIn) == loginId && loggedIn == pap.Active.Id;
		[HttpPost("GiveUp")]
		public async Task GiveUp([FromBody] Shared.Data.User player)
		{
			if (login.Get(player.U) == player.I)
			{
				_ = pap.Do(new GiveUp(pap.Player(player.U)));
				if (pap.Active.Id == player.U)
				{
					_ = await end.NextTurn();
				}
			}
		}
		[HttpPost("MoveInfo")]
		public Shared.Data.MoveInfo MoveInfo([FromBody] Shared.Data.IntPair move)
		{
			var (from, to) = move;
			var possible = pap.Province(from).SoldierTypes.Any(type => type.CanMoveAlone(pap.PaP, from, to));
			var types = pap.Province(from).SoldierTypes.Select(type => type.Description).ToArray();
			return new Shared.Data.MoveInfo(possible, !pap.Province(from).IsAllyOf(pap.Province(to)), pap.Province(to).Occupied, pap.Province(from).Name, pap.Province(to).Name, pap.Province(from).Soldiers.ToString(), pap.Province(to).Soldiers.ToString(), types);
		}
		[HttpPost("Move")]
		public Client.Pages.Move.Errors Move([FromBody] Shared.Data.MoveCmd m)
		{
			if (!Validate(m.LoggedIn, m.LoginId))
			{
				return Client.Pages.Move.Errors.NotPlaying;
			}
			var s = new Soldiers(m.Counts.Select((count, i) => (pap.Province(m.From).Soldiers[i].Type, count)));
			var move = new Move(pap.Player(m.LoggedIn), pap.Province(m.From), pap.Province(m.To), new Army(s, pap.Player(m.LoggedIn)));
			return pap.Do(move) switch
			{
				true => Client.Pages.Move.Errors.Ok,
				false when !pap.Province(m.From).Soldiers.Contains(s) => Client.Pages.Move.Errors.FewSoldiers,
				false when s.Capacity(pap.PaP, m.From, m.To) < 0 => Client.Pages.Move.Errors.LittleCapacity,
				_ => Client.Pages.Move.Errors.Else
			};
		}
		[HttpPost("PurchaseInfo")]
		public Shared.Data.PurchaseInfo PurchaseInfo([FromBody] Shared.Data.IntPair purchase)
		{
			var (player, land) = purchase;
			return new Shared.Data.PurchaseInfo(new Buy(pap.Player(player), pap.Province(land), 0).Allowed(pap.PaP), pap.Province(land).Name, (pap.Province(land) as Land)?.Price ?? int.MaxValue, pap.Player(player).Money);
		}
		[HttpPost("Purchase")]
		public void Purchase([FromBody] Shared.Data.PurchaseCmd purchase)
		{
			if (Validate(purchase.LoggedIn, purchase.LoginId) && pap.Province(purchase.Land) is Land Land)
			{
				if (Land.Price > pap.Player(purchase.LoggedIn).Money)
				{
					_ = pap.Do(new Borrow(pap.Player(purchase.LoggedIn), Land.Price - pap.Player(purchase.LoggedIn).Money, sl.Settings));
				}
				_ = pap.Do(new Buy(pap.Player(purchase.LoggedIn), pap.Province(purchase.Land), Land.Price));
			}
		}
		[HttpPost("RecruitInfo")]
		public Shared.Data.RecruitInfo RecruitInfo([FromBody] Shared.Data.IntPair p)
		{
			return new Shared.Data.RecruitInfo(pap.Province(p.A).Name, pap.Province(p.A).Soldiers.ToString(), sl.Settings.RecruitableTypes(pap.Province(p.A)).Select(t => new Shared.Data.SoldiersItem(t.Description, t.Price)).ToArray(), pap.Player(p.B).Money);
		}
		[HttpPost("Recruit")]
		public void Recruit([FromBody] Shared.Data.RecruitCmd r)
		{
			if (Validate(r.LoggedIn, r.LoginId))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => (sl.Settings.SoldierTypes[i], count)));
				if (soldiers.Price > pap.Player(r.LoggedIn).Money)
				{
					_ = pap.Do(new Borrow(pap.Player(r.LoggedIn), soldiers.Price - pap.Player(r.LoggedIn).Money, sl.Settings));
				}
				_ = pap.Do(new Recruit(pap.Player(r.LoggedIn), pap.Province(r.Province), soldiers));
			}
		}
		[HttpPost("Donate")]
		public bool Donate([FromBody] Shared.Data.DonationCmd donation)
		{
			return login.Get(donation.LoggedIn) == donation.LoginId && pap.Do(new Donate(pap.Player(donation.LoggedIn), pap.Player(donation.Recipient), donation.Amount));
		}
	}
}