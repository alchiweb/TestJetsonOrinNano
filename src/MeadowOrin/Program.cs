using System.Threading.Tasks;
using Meadow;

namespace MeadowOrin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await MeadowOS.Start(args);
        }
    }
}