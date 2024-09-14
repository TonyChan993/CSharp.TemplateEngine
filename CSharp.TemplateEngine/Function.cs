using System;
using System.Collections.Generic;

namespace CSharp.TemplateEngine
{
	/// <summary>
	/// A condition in templates.
	/// </summary>
	sealed class Function : IRenderizable
	{
		public string Name { get; set; }		
		public List<string> Arguments { get; set; }

		/// <summary>
		/// The value assigned from outside as a result of evaluating the function.
		/// This value will appear in place of the function when Render is executed.
		/// </summary>
		public string Value { get; set; }	

		public void Render(RenderContext context)
		{
			object value = this.Value;
			if(value == null)
			{
				value = Evaluator.Eval(this.Name, this.Arguments, context.RenderModel, context);
			}

			context.Writer.Write(value);
		}
	}
}

