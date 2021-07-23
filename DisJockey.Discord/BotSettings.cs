namespace DisJockey.Discord
{
    public class BotSettings
    {
        public ulong OwnerId { get; set; }
        public string BotToken { get; set; }
        public char Prefix { get; set; }
        public string LavalinkHost { get; set; }
        public ushort LavalinkPort { get; set; }
        public string LavalinkPassword { get; set; }
        public bool LavalinkIsSSL { get; set; }
    }
}