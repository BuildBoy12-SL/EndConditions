namespace EndConditions
{
    using Exiled.API.Enums;
    using System.Collections.Generic;

    public readonly struct Condition
    {
        public Condition(List<string> escapeConditions, LeadingTeam leadingTeam, string name, List<string> roleConditions)
        {
            EscapeConditions = escapeConditions;
            LeadingTeam = leadingTeam;
            Name = name;
            RoleConditions = roleConditions;
        }

        public List<string> EscapeConditions { get; }
        public LeadingTeam LeadingTeam { get; }
        public string Name { get; }
        public List<string> RoleConditions { get; }
    }
}