﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dyalect.Util
{
    internal static class HelpGenerator
    {
        private const int HELP_LENGTH = 60;

        public static string Generate<T>()
        {
            var sb = new StringBuilder();
            var names = new List<string>();
            var helps = new List<string>();
            var props = typeof(T).GetProperties().Where(p => Attribute.IsDefined(p, typeof(BindingAttribute)));

            foreach (var ac in props)
            {
                var attr = Attribute.GetCustomAttribute(ac, typeof(BindingAttribute)) as BindingAttribute;

                var ln = new List<string>();

                foreach (var n in attr.Names)
                    ln.Add(n);

                var name = string.Join(", ", ln.Select(n => "-" + n));

                if (string.IsNullOrEmpty(name))
                    name = "<default>";

                var help = attr.Help ?? "";
                names.Add(name);
                helps.Add(help);
            }

            var pad = names.Select(s => s.Length).Max();

            for (var i = 0; i < names.Count; i++)
            {
                sb.AppendFormat("{0}{1}    ", names[i], new string(' ', pad - names[i].Length));
                var h = helps[i];
                SplitHelp(sb, pad, h);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void SplitHelp(StringBuilder sb, int pad, string h)
        {
            if (h.Length > HELP_LENGTH)
            {
                var idx = 0;
                var lastIdx = 0;
                while ((idx = h.IndexOf(' ', lastIdx + 1)) < HELP_LENGTH && idx != -1)
                    lastIdx = idx;
                lastIdx = lastIdx == 0 ? idx : lastIdx;

                if (lastIdx > 0)
                {
                    var ch = h.Substring(0, lastIdx);
                    h = h.Substring(lastIdx).Trim();

                    sb.AppendLine(ch);
                    sb.Append(new string(' ', pad + 4));
                    SplitHelp(sb, pad, h);
                }
                else
                    sb.AppendLine(h);
            }
            else
                sb.AppendLine(h);
        }
    }
}