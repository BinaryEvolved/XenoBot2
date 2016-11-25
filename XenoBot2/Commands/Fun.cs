using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using XenoBot2.Data;
using XenoBot2.Shared;
using Newtonsoft.Json.Linq;

namespace XenoBot2.Commands
{
    internal static class Fun
    {
        private static Random _rnd;

        internal static async Task WhatTheCommit(CommandInfo info, User author, Channel channel)
        {
            if (_rnd == null)
                _rnd = new Random();

            Utilities.WriteLog(author, "requested a WhatTheCommit message.");
            // TODO: Not this
            var msg = Strings.WhatTheCommit.GetRandom().Trim()
                .Replace("XNAMEX", Strings.Names.GetRandom())
                .Replace("XUPPERNAMEX", Strings.Names.GetRandom().ToUpper())
                .Replace("XNUMX", _rnd.Next(9000).ToString())
                .Replace("<br/>", "\n");
            await channel.SendMessage($"*{msg}*");
        }

        internal static async Task CatFact(CommandInfo info, User author, Channel channel)
        {
            Utilities.WriteLog(author, "requested a cat fact.");
            await
                channel.SendMessage(
                    $"Cat Fact: *{await Shared.Utilities.GetStringAsync("https://cat-facts-as-a-service.appspot.com/fact")}*");
        }

        internal static async Task EightBall(CommandInfo info, User author, Channel channel)
        {
            Utilities.WriteLog(author, "consulted the 8 ball.");
            if (info.HasArguments)
            {
                await channel.SendMessage($"{author.NicknameMention} asked: **{string.Join(" ", info.Arguments)}**\n" +
                                          $"The 8 Ball says... *{Strings.EightBall.GetRandom()}*");
            }
            else
            {
                await channel.SendMessage($"*{Strings.EightBall.GetRandom()}*");
            }
        }

        internal static async Task RandomCat(CommandInfo info, User author, Channel channel)
        {
            using (var client = new WebClient())
            {
                dynamic result = JObject.Parse(await client.DownloadStringTaskAsync("http://random.cat/meow"));
                Utilities.WriteLog(author, "requested a random cat.");
                await channel.SendMessage($"{author.NicknameMention}: {result.file}");
            }
        }
    }
}
