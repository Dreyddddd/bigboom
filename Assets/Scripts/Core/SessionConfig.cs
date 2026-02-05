namespace BigBoom.Core
{
    public readonly struct SessionConfig
    {
        public SessionConfig(int rounds, int bots, GameModeType mode)
        {
            Rounds = rounds;
            Bots = bots;
            Mode = mode;
        }

        public int Rounds { get; }
        public int Bots { get; }
        public GameModeType Mode { get; }
    }
}
