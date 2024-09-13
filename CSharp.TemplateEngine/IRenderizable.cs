using System;
using System.IO;
using System.Collections.Generic;

namespace CSharp.TemplateEngine
{	
	public interface IRenderizable
	{
		void Render(RenderContext context);
	}

	public interface IContainer
	{
		List<IRenderizable> Items { get; }
	}
}

