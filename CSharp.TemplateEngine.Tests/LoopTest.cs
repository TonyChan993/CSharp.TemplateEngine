namespace CSharp.TemplateEngine.Tests;

public class LoopTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public static void ParseForeach()
	{
		var template = Template.Parse("Models:{{ foreach(item in .) }} {{.item}}{{end}}");
		Assert.AreEqual("Models: Padelclick Golfgest", template.Render(new string[] { "Padelclick", "Golfgest" }));
	}
	
	[Test]
	public static void ParseForeach2()
	{
		var list = new List<UserModel>()
		{
			new UserModel()
			{
				Age = 13,
				UserName = "Charlie",
				Phone = "159"
			},
			new UserModel()
			{
				Age = 6,
				UserName = "Cindy",
				Phone = "159"
			}
		};
		var template = Template.Parse("Models:{{ foreach(item in .) }} {{.item.UserName}}{{end}}");
		Assert.AreEqual("Models: Charlie Cindy", template.Render(list));
	}

	[Test]
	public static void ParseForeachIndex()
	{
		var template = Template.Parse("Models:{{ foreach(item in .items) }} {{.item}}{{end}}");
		var model = new { items = new string[] { "Padelclick", "Golfgest" } };
		var value = template.Render(model);
		Assert.AreEqual(value, "Models: Padelclick Golfgest");
	}

	[Test]
	public static void ParseForeachIndex2()
	{
		var template = Template.Parse("Models:{{ foreach(item, i in .items) }} {{.i}}{{end}}");
		var model = new { items = new string[] { "Padelclick", "Golfgest" } };
		var value = template.Render(model);
		Assert.AreEqual(value, "Models: 0 1");
	}

	[Test]
	public static void ParseForeachIndex3()
	{
		var template = Template.Parse("Models:{{ foreach(item, i in .items) }} {{.item}}{{.i}}{{end}}");
		var model = new { items = new string[] { "Padelclick", "Golfgest" } };
		var value = template.Render(model);
		Assert.AreEqual(value, "Models: Padelclick0 Golfgest1");
	}

	[Test]
	public static void ParseForeachIndex4()
	{
		var template = Template.Parse("Models:{{ foreach(item, i in .items) }} {{.item}}{{.i}}{{end}}");
		var model = new { items = new string[] { "Padelclick", "Golfgest" } };
		var value = template.Render(model);
		Assert.AreEqual(value, "Models: Padelclick0 Golfgest1");
	}

	[Test]
	public static void ParseNestedForeach()
	{
		var template = Template.Parse(@"Organization:{{ foreach(org in .) }}
                                                {{.org.country}}:{{ foreach(worker in .org.workers) }} {{.worker.name}}{{end}}
                                            {{end}}");

		var model = new object[]
		{
			new { country = "Spain", workers = new object[] { new { name = "Juan" }, new { name = "Luis" } } },
			new { country = "UK", workers = new object[] { new { name = "John" }, new { name = "Bill" } } }
		};

		var value = template.Render(model);

		var expected = @"Organization:
                                                Spain: Juan Luis
                                            
                                                UK: John Bill
                                            ";

		Assert.AreEqual(value, expected);
	}
}