using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IConfig
	{
		Shape[] Shapes { get; }
		Graph Graph { get; }
		Settings Settings { get; }
	}
	public class Config : IConfig
	{
		public Shape[] Shapes { get; }
		public Graph Graph { get; }
		public Settings Settings { get; }
		public Config(IFile shapes, IFile graph, IFile settings)
		{
			Shapes = shapes.Read<Shape[]>();
			Graph = graph.Read<Graph>();
			Settings = settings.Read<Settings>();
		}
	}
}
