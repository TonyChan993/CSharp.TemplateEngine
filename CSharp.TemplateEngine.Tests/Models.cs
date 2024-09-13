namespace CSharp.TemplateEngine.Tests
{
    public class UserModel
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }

        public string Hello()
        {
            return $"Hello,I am {UserName}.";
        }
    }
}