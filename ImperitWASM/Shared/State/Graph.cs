using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public record Graph(ImmutableArray<ImmutableArray<int>> Neighbors)
	{
		public int NeighborCount(int vertex) => Neighbors[vertex].Length;
		public ImmutableArray<int> this[int vertex] => Neighbors[vertex];
		public int Length => Neighbors.Length;
		public ImmutableArray<ImmutableArray<int>>.Enumerator GetEnumerator() => Neighbors.GetEnumerator();
		public bool Passable(int from, int to, int limit, Func<int, int, int> difficulty)
		{
			var stack = new List<(int Pos, int Distance)>() { (from, 0) };
			bool[]? visited = new bool[Length];
			visited[from] = true;
			for (int i = 0; i < stack.Count; ++i)
			{
				if (stack[i].Pos == to)
				{
					return true;
				}
				foreach (int vertex in this[stack[i].Pos].Where(n => !visited[n]))
				{
					if (stack[i].Distance + difficulty(stack[i].Pos, vertex) <= limit)
					{
						stack.Add((vertex, stack[i].Distance + difficulty(stack[i].Pos, vertex)));
						visited[vertex] = true;
					}
				}
			}
			return false;
		}
	}
}