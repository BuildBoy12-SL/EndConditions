namespace EndConditions
{
    using Exiled.API.Features;
    using System.Collections.Generic;
    
    public static class API
    {
        /// <summary>
        /// Any player in this list will be ignored when checking roles.
        /// </summary>
        public static readonly List<Player> BlacklistedPlayers = new List<Player>();
        
        /// <summary>
        /// Any player in this dictionary will have the role specified by the string rather than by their in-game role.
        /// </summary>
        public static readonly Dictionary<Player, string> ModifiedRoles = new Dictionary<Player, string>();
    }
}