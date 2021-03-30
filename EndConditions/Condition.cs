namespace EndConditions
{
    using System.Collections.Generic;
    using Exiled.API.Enums;

    public class Condition
    {
        public List<string> EscapeConditions { get; set; }

        public LeadingTeam LeadingTeam { get; set; }

        public string Name { get; set; }

        public List<string> RoleConditions { get; set; }
    }
}