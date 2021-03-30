namespace EndConditions
{
    using System.Collections.Generic;
    using Exiled.API.Features;

    public static class API
    {
        /// <summary>
        /// Gets a list of players to be ignored while checking roles.
        /// </summary>
        public static List<Player> BlacklistedPlayers { get; } = new List<Player>();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> where any <see cref="Player"/> as a key will have the role specified by the <see cref="string"/> rather than by their in-game role.
        /// </summary>
        public static Dictionary<Player, string> ModifiedRoles { get; } = new Dictionary<Player, string>();
    }
}