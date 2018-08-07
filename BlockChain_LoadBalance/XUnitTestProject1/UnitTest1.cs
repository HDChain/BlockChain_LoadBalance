using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            using (var c = new HttpClient()) {
                 var str = await c.GetAsync("http://127.0.0.1:50000/api/default/Test");
                 
                 Xunit.Assert.Equal("ok", await str.Content.ReadAsStringAsync());
            }
        }


    }
}
