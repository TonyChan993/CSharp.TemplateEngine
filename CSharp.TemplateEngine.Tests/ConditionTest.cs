namespace CSharp.TemplateEngine.Tests;

public class ConditionTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public static void ParseIf()
	{
		var template = Template.Parse("IsAdmin:{{ if equals(.isAdmin, true) }}yes!{{ end }}");
		Assert.AreEqual(template.Render(new { isAdmin = true }), "IsAdmin:yes!");
	}

	[Test]
	public static void IfWithModelKey()
	{
		var template = Template.Parse("IsAdmin:{{ if .isAdmin }} yes!{{ end }}");
		Assert.AreEqual(template.Render(new { isAdmin = true }), "IsAdmin: yes!");
	}

	[Test]
	public static void ParseIfElse()
	{
		var template = Template.Parse("IsAdmin:{{ if equals(.isAdmin, true) }}yes!{{ else }}no!{{ end }}");
		Assert.AreEqual(template.Render(new { isAdmin = false }), "IsAdmin:no!");
	}

	[Test]
	public static void ParseIf2()
	{
		var template = Template.Parse("IsGolf:{{ if equals(., Golfgest) }}yes!{{end}}");
		Assert.AreEqual(template.Render("Golfgest"), "IsGolf:yes!");
	}
	
	[Test]
	public static void ParseIf3()
	{
		var template = Template.Parse("Age:{{ if GreaterThan(.Age, 18) }} Adult!{{ end }}");
		Assert.AreEqual(template.Render(new { Age = 20 }), "Age: Adult!");
	}
}