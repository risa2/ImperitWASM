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
		readonly IActionLoader actions;
		readonly ILoginService login;
		readonly IActivePlayer active;
		readonly IPlayersLoader players;
		readonly IProvincesLoader provinces;
		readonly ISettingsLoader sl;
		public ActionController(IActionLoader actions, ILoginService login, IActivePlayer active, IPlayersLoader players, IProvincesLoader provinces, ISettingsLoader sl)
		{
			this.actions = actions;
			this.login = login;
			this.active = active;
			this.players = players;
			this.provinces = provinces;
			this.sl = sl;
		}
		bool Validate(int loggedIn, string loginId) => login.Get(loginId) == loggedIn && loggedIn == active.Id;
		[HttpGet("Debts")]
		public IEnumerable<Shared.Data.DebtInfo> Debts() => actions.OfType<Loan>().Select(l => new Shared.Data.DebtInfo(l.Debtor, l.Debt));
		[HttpPost("GiveUp")]
		public void GiveUp([FromBody] Shared.Data.User player)
		{
			if (login.Get(player.LoginId) == player.Id)
			{
				actions.Add(new GiveUp(player.Id));
				if (active.Id == player.Id)
				{
					active.Next(players);
				}
			}
		}
		[HttpPost("MoveInfo")]
		public Shared.Data.MoveInfo MoveInfo([FromBody] Shared.Data.IntPair move)
		{
			var (from, to) = move;
			var possible = provinces[from].SoldierTypes.Any(type => type.CanMoveAlone(provinces, from, to));
			var types = provinces[from].SoldierTypes.Select(type => type.Description).ToArray();
			return new Shared.Data.MoveInfo(possible, !provinces[from].IsAllyOf(provinces[to]), provinces[to].Occupied, provinces[from].Name, provinces[to].Name, provinces[from].Soldiers.ToString(), provinces[to].Soldiers.ToString(), types);
		}
		[HttpPost("Move")]
		public Client.Pages.Move.Errors Move([FromBody] Shared.Data.MoveCmd m)
		{
			if (!Validate(m.LoggedIn, m.LoginId))
			{
				return Client.Pages.Move.Errors.NotPlaying;
			}
			var s = new Soldiers(m.Counts.Select((count, i) => (provinces[m.From].Soldiers[i].Type, count)));
			if (!provinces[m.From].Soldiers.Contains(s))
			{
				return Client.Pages.Move.Errors.FewSoldiers;
			}
			if (s.Capacity(provinces, m.From, m.To) < 0)
			{
				return Client.Pages.Move.Errors.LittleCapacity;
			}
			var army = new Army(s, players[m.LoggedIn]);
			return actions.Add(m.IsAttack ? new Attack(m.LoggedIn, m.From, m.To, army) as Move : new Reinforce(m.LoggedIn, m.From, m.To, army))
				? Client.Pages.Move.Errors.Ok : Client.Pages.Move.Errors.Else;
		}
		[HttpPost("PurchaseInfo")]
		public Shared.Data.PurchaseInfo PurchaseInfo([FromBody] Shared.Data.IntPair purchase)
		{
			var (player, land) = purchase;
			return new Shared.Data.PurchaseInfo(new Buy(players[player], land, 0).Allowed(players, provinces), provinces[land].Name, (provinces[land] as Land)?.Price ?? int.MaxValue, players[player].Money);
		}
		[HttpPost("Purchase")]
		public void Purchase([FromBody] Shared.Data.PurchaseCmd purchase)
		{
			if (Validate(purchase.LoggedIn, purchase.LoginId) && provinces[purchase.Land] is Land Land)
			{
				if (Land.Price > players[purchase.LoggedIn].Money)
				{
					actions.Add(new Borrow(purchase.LoggedIn, Land.Price - players[purchase.LoggedIn].Money, sl.Settings));
				}
				actions.Add(new Buy(players[purchase.LoggedIn], purchase.Land, Land.Price));
			}
		}
		[HttpPost("RecruitInfo")]
		public Shared.Data.RecruitInfo RecruitInfo([FromBody] int i)
		{
			return new Shared.Data.RecruitInfo(provinces[i].Name, provinces[i].Soldiers.ToString(), sl.Settings.SoldierTypes.Where(t => t.IsRecruitable(provinces[i])).Select(t => new Shared.Data.SoldiersItem(t.Description, t.Price)).ToArray(), players[provinces[i].Army.PlayerId].Money);
		}
		[HttpPost("Recruit")]
		public void Recruit([FromBody] Shared.Data.RecruitCmd r)
		{
			if (Validate(r.LoggedIn, r.LoginId))
			{
				var soldiers = new Soldiers(r.Counts.Select((count, i) => (sl.Settings.SoldierTypes[i], count)));
				if (soldiers.Price > players[r.LoggedIn].Money)
				{
					actions.Add(new Borrow(r.LoggedIn, soldiers.Price - players[r.LoggedIn].Money, sl.Settings));
				}
				actions.Add(new Recruit(r.LoggedIn, r.Province, new Army(soldiers, players[r.LoggedIn])));
			}
		}
		[HttpPost("Donate")]
		public bool Donate([FromBody] Shared.Data.DonationCmd donation)
		{
			if (login.Get(donation.LoginId) == donation.LoggedIn)
			{
				return actions.Add(new Donate(donation.LoggedIn, donation.Recipient, donation.Amount));
			}
			return false;
		}
	}
}
