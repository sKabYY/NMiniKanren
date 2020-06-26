using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nccc.Parser;
using System;
using System.Reflection;

namespace NMiniKanren.G.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private NcParser parser = NcParser.LoadFromAssembly(
            Assembly.GetExecutingAssembly(),
            "NMiniKanren.G.Tests.kanren.grammer"); 
        [TestMethod]
        public void TestMethod1()
        {
            var src = @"
run * for (x y)
|- x == 1 || x == 2 || x == 3
|- y == 'a' || y == 'b'
";
            var res = parser.Parse(src);
            Console.WriteLine(res.ToSExp().ToPrettyString());
        }
    }
}
