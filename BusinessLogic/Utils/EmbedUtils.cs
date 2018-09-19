using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;

namespace BusinessLogic.Utils
{
    public static class EmbedUtils
    {
        public static DiscordEmbed GetEmbed(string fieldName, IEnumerable<string> fieldValues)
        {
            var emberBuilder = new DiscordEmbedBuilder();
            var fieldValue = fieldValues.Aggregate((s1, s2) => $"{s1}{Environment.NewLine}{s2}");

            emberBuilder.AddField(fieldName, fieldValue);
            var embed = emberBuilder.Build();

            return embed;
        }
    }
}
