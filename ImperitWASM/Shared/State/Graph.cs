using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.GraphConverter))]
	public record Graph(ImmutableArray<int> Edges, ImmutableArray<int> Starts) : IReadOnlyList<IEnumerable<int>>
	{
		public bool Passable(int from, int to, int limit, Func<int, int, int> difficulty)
		{
			var stack = new List<(int Pos, int Distance)>() { (from, 0) };
			bool[]? visited = new bool[Count];
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
		public int NeighborCount(int vertex) => Starts[vertex + 1] - Starts[vertex];
		public IEnumerable<int> this[int vertex] => Edges.Take(Starts[vertex + 1]).Skip(Starts[vertex]);
		public int Count => Starts.Length - 1;
		public IEnumerator<IEnumerable<int>> GetEnumerator() => Enumerable.Range(0, Count).Select(i => this[i]).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}