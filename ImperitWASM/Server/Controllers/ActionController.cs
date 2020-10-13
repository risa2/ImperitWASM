using ImperitWASM.Server.Services;
using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.Motion.Commands;
using ImperitWASM.Shared.State;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ActionController : ControllerBase
	{
		readonly IPlayersProvinces pap;
		readonly ILoginService login;
		readonly ISettingsLoader sl;
		public ActionController(IPlayersProvinces pap, ILoginService login, ISettingsLoader sl)
		{
			this.pap = pap;
			this.login = login;
			this.sl = sl;
		}
		bool Validate(int loggedIn, string loginId) => login.Get(loginId) == loggedIn && loggedIn == pap.Active.Id;
		[HttpGet("Debts")]
		public IEnumerable<Shared.Data.DebtInfo> Debts() => pap.Players.SelectMany(p => p.Actions.OfType<Loan>().Select(l => new Shared.Data.DebtInfo(p.Id, l.Debt)));
		[HttpPost("GiveUp")]
		public void GiveUp([FromBody] Shared.Data.User player)
		{
			if (login.Get(player.LoginId) == player.Id)
			{
				pap.Do(new GiveUp(pap.Player(player.Id)));
				if (pap.Active.Id == player.Id)
				{
					pap.Next();
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
			if (!pap.Province(m.From).Soldiers.Contains(s))
			{
				return Client.Pages.Move.Errors.FewSoldiers;
			}
			if (s.Capacity(pap.PaP, m.From, m.To) < 0)
			{
				return Client.Pages.Move.Errors.LittleCapacity;
			}
			var army = new Army(s, pap.Player(m.LoggedIn));
			return pap.Do(m.IsAttack ? new Attack(pap.Player(m.LoggedIn), pap.Province(m.From), pap.Province(m.To), army) as Move : new Reinforce(pap.Player(m.LoggedIn), pap.Province(m.From), pap.Province(m.To), army))
				? Client.Pages.Move.Errors.Ok : Client.Pages.Move.Errors.Else;
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
					pap.Do(new Borrow(pap.Player(purchase.LoggedIn), Land.Price - pap.Player(purchase.LoggedIn).Money, sl.Settings));
				}
				pap.Do(new Buy(pap.Player(purchase.LoggedIn), pap.Province(purchase.Land), Land.Price));
			}
		}
		[HttpPost("RecruitInfo")]
		public Shared.Data.RecruitInfo RecruitInfo([FromBody] int i)
		{
			return new Shared.Data.RecruitInfo(pap.Province(i).Name, pap.Province(i).Soldiers.ToString(), sl.Settings.SoldierTypes.Where(t => t.IsRecruitable(pap.Province(i))).Select(t => new Shared.Data.SoldiersItem(t.Description, t.Price)).ToArray(), pap.Province(i).Army.Player.Money);
		}
		[HttpPost("Recruit")]
		public void Recruit([FromBody] Shared.Data.RecruitCmd r)
		{
			if (Validate(r.LoggedIn, r.LoginId))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => (sl.Settings.SoldierTypes[i], count)));
				if (soldiers.Price > pap.Player(r.LoggedIn).Money)
				{
					pap.Do(new Borrow(pap.Player(r.LoggedIn), soldiers.Price - pap.Player(r.LoggedIn).Money, sl.Settings));
				}
				pap.Do(new Recruit(pap.Player(r.LoggedIn), pap.Province(r.Province), soldiers));
			}
		}
		[HttpPost("Donate")]
		public bool Donate([FromBody] Shared.Data.DonationCmd donation)
		{
			if (login.Get(donation.LoginId) == donation.LoggedIn)
			{
				return pap.Do(new Donate(pap.Player(donation.LoggedIn), pap.Player(donation.Recipient), donation.Amount));
			}
			return false;
		}
	}
}