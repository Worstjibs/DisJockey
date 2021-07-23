using System.Threading.Tasks;
using Discord.Commands;

namespace DisJockey.Discord.Modules {
    public class Ping : ModuleBase<SocketCommandContext> {
        [Command("Ping")]
        public async Task Pong() {
            await ReplyAsync("Pong!");
        }
    }
}