namespace CSharp.TemplateEngine.Tests;

public class FunctionTest
{
	[SetUp]
	public void Setup()
	{
	}


	[Test]
	public static void TestGenericFunc3()
	{
		var template = Template.Parse("{{ pow(.) }}");
		template.AddFunction("pow", (int a) => a * a);
		Assert.AreEqual("9", template.Render(3));
	}

	[Test]
	public static void TestGenericFunc33()
	{
		var template = Template.Parse("{{ pow(3) }}");
		template.AddFunction("pow", (int a) => a * a);
		Assert.AreEqual("9", template.Render());
	}

	[Test]
	public static void TestGenericFunc34()
	{
		var template = Template.Parse("{{ sum(3, 8) }}");
		template.AddFunction("sum", (int a, int b) => a + b);
		Assert.AreEqual("11", template.Render());
	}

	[Test]
	public static void TestGenericFunc4()
	{
		var template = Template.Parse("{{ sum(3, 8) }}");
		template.AddFunction("sum", (int a, int b, RenderContext c) =>
		{
			Assert.NotNull(c);
			return a + b;
		});
		Assert.AreEqual("11", template.Render());
	}

	[Test]
	public static void TestGenericFunc5()
	{
		var template = Template.Parse("{{ foo(3, 8, 9, 10, 'ssss') }}");
		template.AddFunction("foo", (int a, int b, string[] args) => a + b);
		Assert.AreEqual("11", template.Render());
	}

	[Test]
	public static void TestGenericFunc6()
	{
		var template = Template.Parse("{{ foo(3, 8, 9, 10, 'ssss') }}");
		template.AddFunction("foo", (int a, string[] args) => { return args.Length; });
		Assert.AreEqual("4", template.Render());
	}

	[Test]
	public static void TestGenericFunc7()
	{
		var template = Template.Parse("{{ foo('alaslas', 3, 'ssss') }}");
		template.AddFunction("foo", (object a, string[] args) => { return args.Length; });
		Assert.AreEqual("2", template.Render());
	}

	[Test]
	public static void TestGenericFunc8()
	{
		var template = Template.Parse("{{ foo('alaslas', 3, 'ssss') }}");
		template.AddFunction("foo", (string[] args) => { return args.Length; });
		Assert.AreEqual("3", template.Render());
	}

	[Test]
	public static void TestGenericFunc9()
	{
		var template = Template.Parse("{{ foo(.a, .b, 3) }}");
		template.AddFunction("foo", (string[] args) => { return string.Concat(args); });
		Assert.AreEqual("123", template.Render(new { a = "1", b = "2" }));
	}
	
	[Test]
	public static void TestGenericFunc10()
	{
		var user = new UserModel()
		{
			UserName = "Bill",
			Age = 44,
			Phone = "911"
		};
		var template = Template.Parse("{{ Hello() }}");
		template.AddFunction("Hello", user.Hello);
		Assert.AreEqual("Hello,I am Bill.", template.Render(user));
	}

}