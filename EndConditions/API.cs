namespace EndConditions
{
    using Exiled.API.Features;
    using System.Collections.Generic;

    public static class API
    {
        /// <summary>
        /// Any player in this list will be ignored when checking roles.
        /// </summary>
        public static List<Player> BlacklistedPlayers { get; } = new();

        /// <summary>
        /// Any player in this dictionary will have the role specified by the string rather than by their in-game role.
        /// </summary>
        public static Dictionary<Player, string> ModifiedRoles { get; } = new();
    }
}