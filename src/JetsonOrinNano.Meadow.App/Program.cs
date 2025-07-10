using System.Threading.Tasks;
using Meadow;

namespace JetsonOrinNano.Meadow.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await MeadowOS.Start(args);
        }
    }
}