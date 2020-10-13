using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Server.Load
{
	public class JsonAction : IEntity<IAction, (Settings, IReadOnlyList<Player>)>
	{
		public string? Type { get; set; }
		public JsonArmy? Army { get; set; }
		public JsonSoldiers? Soldiers { get; set; }
		public int? Debt { get; set; }
		public IAction Convert(int i, (Settings, IReadOnlyList<Player>) arg)
		{
			var (settings, players) = arg;
			return Type switch
			{
				"Reinforcement" => new Reinforcement(Soldiers!.Convert(i, settings.SoldierTypes)),
				"Battle" => new Fight(Army!.Convert(i, (players, settings.SoldierTypes))),
				"Earnings" => new Earnings(),
				"Instability" => new Instability(),
				"Loan" => new Loan(Debt ?? 0, settings),
				"Mortality" => new Mortality(),
				_ => throw new System.Exception("Unknown type of Action: " + Type)
			};

		}
		public static JsonAction From(IAction action)
		{
			return action switch
			{
				Reinforcement Add => new JsonAction { Type = "Reinforcement", Soldiers = JsonSoldiers.From(Add.Soldiers) },
				Fight Attack => new JsonAction { Type = "Battle", Army = JsonArmy.From(Attack.Army) },
				Earnings _ => new JsonAction { Type = "Earnings" },
				Instability _ => new JsonAction { Type = "Instability" },
				Loan Loan => new JsonAction { Type = "Loan", Debt = Loan.Debt },
				Mortality _ => new JsonAction { Type = "Mortality" },
				_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
			};
		}
	}
}