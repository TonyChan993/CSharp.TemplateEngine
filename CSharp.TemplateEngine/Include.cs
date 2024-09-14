using System;

namespace CSharp.TemplateEngine
{
	// A template that is inserted into another template.
	sealed class Include : IRenderizable
	{
		public string Path { get; set; }
		public Template MainTemplate { get; set; }

		public void Render(RenderContext context)
		{
			var template = context.Templates [this.Path];
			if (template != null)
			{
				template.Render(context);
			}
		}
	}
}

