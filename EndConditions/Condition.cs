namespace EndConditions
{
    using Exiled.API.Enums;
    using System.Collections.Generic;

    public class Condition
    {
        public List<string> EscapeConditions { get; set; }
        public LeadingTeam LeadingTeam { get; set; }
        public string Name { get; set; }
        public List<string> RoleConditions { get; set; }
    }
}