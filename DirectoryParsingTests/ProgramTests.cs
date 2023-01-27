using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;

namespace DirectoryParsing.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TryParsePathTest()
        {
            Dictionary<string, string> testValues = new()
            {
                { "Папка в корне", "~/asd" },
                { "Уровень выше в корне и папка", "~/../asd" }
            };

            foreach (var testValue in testValues)
                Assert.IsTrue(Program.PathRegex.IsMatch(testValue.Value), $"{testValue.Key} [{testValue.Value}]");
        }
    }
}