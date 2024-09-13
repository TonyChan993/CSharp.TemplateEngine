
namespace CSharp.TemplateEngine
{
	// Una cadena de texto
	sealed class TextChunk : IRenderizable
	{
		public string Value { get; set; }

		public void Render(RenderContext context)
		{
			context.Writer.Write (this.Value);
		}
	}
}
