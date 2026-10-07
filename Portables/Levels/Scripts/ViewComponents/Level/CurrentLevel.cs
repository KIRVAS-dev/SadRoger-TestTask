namespace ViewComponents.Level
{
    public sealed class CurrentLevel
    {
        public Level Level { get; private set; }

        internal void Set(Level level)
        {
            Level = level;
        }
    }
}
