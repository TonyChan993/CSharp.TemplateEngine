
namespace CSharp.TemplateEngine
{
	/// <summary>
	/// A string of text.
	/// </summary>
	sealed class TextChunk : IRenderizable
	{
		public string Value { get; set; }

		public void Render(RenderContext context)
		{
			context.Writer.Write (this.Value);
		}
	}
}
