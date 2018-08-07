using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using XUnitTestProject1.Models;

namespace XUnitTestProject1.Eth {
    public class RpcTest {
        public RpcTest(ITestOutputHelper output) {
            this.output = output;
        }

        private readonly ITestOutputHelper output;

        [Fact]
        public async Task Test1() {
            using (var c = new HttpClient()) {
                var resp = await c.PostAsync("http://127.0.0.1:50000/ethrpc/1",
                    new StringContent(
                        JsonConvert.SerializeObject(new JsonRpcReq {
                            Method = "web3_clientVersion"
                        }),
                        Encoding.UTF8,
                        "application/json"));


                var content = await resp.Content.ReadAsStringAsync();

                output.WriteLine("This is output from {0}", content);
            }
        }
    }
}