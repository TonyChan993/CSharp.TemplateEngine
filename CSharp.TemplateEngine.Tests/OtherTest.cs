namespace CSharp.TemplateEngine.Tests;

public class OtherTest
{
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public static void TestInclude()
    {
        var template = Template.Parse("{{ include header }} Its {{.time}}.");
        template.ParseInclude("header", "Hello {{.name}}!");
        var render = template.Render(new Dictionary<string, object>() {{"name", "Patrick" }, {"time", "10:00"}});
        Assert.AreEqual(render, "Hello Patrick! Its 10:00.");
    }

    [Test]
    public static void TestExtended()
    {
        var template = Template.Parse("{{ extends main }} {{ block name }}Bill{{ end }} ");
        template.ParseInclude("main", "Hello {{ block name }}{{ end }}!");
        Assert.AreEqual("Hello Bill!", template.Render());
    }

    [Test]
    public static void TestExtended2()
    {
        var template = Template.Parse("{{ extends main }} {{ block name }}Bill{{ end }}  {{ block lastname }}{{ .last }}{{ end }}");
        template.ParseInclude("main", "Hello {{ block name }}{{ end }} {{ block lastName }}{{ end }}!");
        Assert.AreEqual("Hello Bill Smith!", template.Render(new { last = "Smith" }));
    }
    
    
    [Test]
    public static void TestRenderValue()
    {
        var template = Template.Parse("{{ .name }}");

        template.RenderValue = (a, b, c) => { 
            c.Writer.Write("Bill"); 
            return true; 
        };

        Assert.AreEqual("Bill", template.Render());
    }

    [Test]
    public static void TestRenderValue2()
    {
        var template = Template.Parse("{{ .name }}");

        template.RenderValue = (a, b, c) => { 
            c.Writer.Write("Bill"); 
            return true; 
        };

        Assert.AreEqual("Bill", template.Render(new { name = "John" }));
    }

    [Test]
    public static void TestRenderValue3()
    {
        var template = Template.Parse("{{ .name }}");
        template.RenderValue = (a, b, c) => false;
        Assert.AreEqual("John", template.Render(new { name = "John" }));
    }
}