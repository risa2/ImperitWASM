using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;
using ImperitWASM.Shared.State.SoldierTypes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImperitWASM.Server.Load
{
	public class JsonSoldierType : IEntity<SoldierType, bool>
	{
		public string Type { get; set; } = "";
		public Description Description { get; set; } = new Description("", "", "");
		public int AttackPower { get; set; }
		public int DefensePower { get; set; }
		public int Weight { get; set; }
		public int Price { get; set; }
		public int? Capacity { get; set; }
		public int? Speed { get; set; }
		public IEnumerable<int>? RecruitPlaces { get; set; }
		public SoldierType Convert(int i, bool _b) => Type switch
		{
			"P" => new Pedestrian(i, Description, AttackPower, DefensePower, Weight, Price),
			"S" when Capacity is int cap => new Ship(i, Description, AttackPower, DefensePower, Weight, Price, cap),
			"E" when Capacity is int cap && Speed is int s => new Elephant(i, Description, AttackPower, DefensePower, Weight, Price, cap, s, RecruitPlaces!.ToImmutableArray()),
			"ES" when Capacity is int cap && Speed is int s => new ElephantShip(i, Description, AttackPower, DefensePower, Weight, Price, cap, s, RecruitPlaces!.ToImmutableArray()),
			_ => throw new System.Exception("Unknown State.SoldierType type: " + Type)
		};
		public static JsonSoldierType From(SoldierType type) => type switch
		{
			Pedestrian P => new JsonSoldierType { Type = "P", Description = P.Description, AttackPower = P.AttackPower, DefensePower = P.DefensePower, Weight = P.Weight, Price = P.Price },
			Ship S => new JsonSoldierType { Type = "S", Description = S.Description, AttackPower = S.AttackPower, DefensePower = S.DefensePower, Weight = S.Weight, Price = S.Price, Capacity = S.Capacity },
			Elephant E => new JsonSoldierType { Type = "E", Description = E.Description, AttackPower = E.AttackPower, DefensePower = E.DefensePower, Weight = E.Weight, Price = E.Price, Capacity = E.Capacity, Speed = E.Speed, RecruitPlaces = E.RecruitPlaces },
			ElephantShip ES => new JsonSoldierType { Type = "ES", Description = ES.Description, AttackPower = ES.AttackPower, DefensePower = ES.DefensePower, Weight = ES.Weight, Price = ES.Price, Capacity = ES.Capacity, Speed = ES.Speed, RecruitPlaces = ES.RecruitPlaces },
			_ => throw new System.Exception("Unknown State.SoldierType type: " + type.GetType())
		};
	}
}
