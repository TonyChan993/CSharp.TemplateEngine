namespace CSharp.TemplateEngine.Tests;

public class ValueTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void ParseValue1()
	{
		var template = Template.Parse("Hello {{.}}!");
		Assert.AreEqual(template.Render("Patrick"), "Hello Patrick!");
	}

	[Test]
	public static void ParseValue2()
	{
		var template = Template.Parse("Hello {{.name}}!");
		Assert.AreEqual(template.Render(new Dictionary<string, object>() { { "name", "Patrick" } }), "Hello Patrick!");
	}

	[Test]
	public static void ParseValue3()
	{
		var template = Template.Parse("Hello {{.customer.name}}!");
		var model = new { customer = new Dictionary<string, object>() { { "name", "Patrick" } } };
		Assert.AreEqual(template.Render(model), "Hello Patrick!");
	}

	[Test]
	public static void ParseValue4()
	{
		var template = Template.Parse("Hello {{ .customer.name }}!");
		Assert.AreEqual(template.Render(new { customer = new { name = "Patrick" } }), "Hello Patrick!");
	}

	[Test]
	public static void ParseValue5()
	{
		var template = Template.Parse("Hello {{.customer['name']}}!");
		var model = new { customer = new Dictionary<string, object>() { { "name", "Patrick" } } };
		Assert.AreEqual(template.Render(model), "Hello Patrick!");
	}

	[Test]
	public static void ParseValue6()
	{
		var template = Template.Parse("Hello {{.customer[2]}}!");
		var model = new { customer = new List<string>() { "juan", "pedro", "luis" } };
		Assert.AreEqual(template.Render(model), "Hello luis!");
	}

	[Test]
	public static void ParseValue7()
	{
		var template = Template.Parse("Hello {{.UserName}}!Age={{.Age}},Phone={{.Phone}}");
		var model = new UserModel()
		{
			UserName = "Tony",
			Age = 30,
			Phone = "110"
		};
		Assert.AreEqual(template.Render(model), "Hello Tony!Age=30,Phone=110");
	}
	
	[Test]
	public static void ParseValue8()
	{
		Template.TagPrefix = "<%";
		Template.TagSuffix = "%>";
		var template = Template.Parse("Hello <%.UserName%>!Age=<%.Age%>,Phone=<%.Phone%>");
		var model = new UserModel()
		{
			UserName = "Tony",
			Age = 30,
			Phone = "110"
		};
		Assert.AreEqual(template.Render(model), "Hello Tony!Age=30,Phone=110");
	}
}