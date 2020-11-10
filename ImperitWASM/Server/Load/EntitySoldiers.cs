﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldiers : IEntity<Soldiers, IReadOnlyList<SoldierType>>
	{
		[Key] public int Id { get; set; }
		public IEnumerable<EntitySoldierPair>? EntitySoldierPairs { get; set; }
		public Soldiers Convert(IReadOnlyList<SoldierType> types)
		{
			return new Soldiers(EntitySoldierPairs.Select(item => item.Convert(types)));
		}
		public static EntitySoldiers From(Soldiers soldiers, IReadOnlyDictionary<SoldierType, int> map)
		{
			return new EntitySoldiers { EntitySoldierPairs = soldiers.Select(s => EntitySoldierPair.From(s, map)) };
		}
	}
}