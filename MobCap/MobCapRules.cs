#nullable disable
using System;
using System.Collections.Generic;

public static class MobCapRules
{
    public sealed class Row
    {
        public Row(int day, int maxStars, int maxWorldLevel)
        {
            Day = day;
            MaxStars = maxStars;
            MaxWorldLevel = maxWorldLevel;
        }

        public int Day { get; }

        public int MaxStars { get; }

        public int MaxWorldLevel { get; }
    }

    public static bool IsLegacy(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return text.IndexOf(':') >= 0
            && text.IndexOf("day", StringComparison.OrdinalIgnoreCase) < 0;
    }

    public static string Format(IReadOnlyList<Row> rows)
    {
        if (rows == null || rows.Count == 0)
        {
            return "";
        }

        var parts = new List<string>();
        for (int i = 0; i < rows.Count; i++)
        {
            Row row = rows[i];
            string starWord = row.MaxStars == 1 ? "star" : "stars";
            parts.Add("day " + row.Day + " = " + row.MaxStars + " " + starWord + " and world " + row.MaxWorldLevel);
        }

        return string.Join("; ", parts);
    }

    public static List<Row> Parse(string text, Action<string> warn)
    {
        var rows = new List<Row>();
        if (string.IsNullOrWhiteSpace(text))
        {
            return rows;
        }

        bool words = text.IndexOf("day", StringComparison.OrdinalIgnoreCase) >= 0;
        string[] segments = words ? text.Split(';') : text.Split(',');
        for (int i = 0; i < segments.Length; i++)
        {
            string segment = segments[i].Trim();
            if (segment.Length == 0)
            {
                continue;
            }

            if (!TryReadRow(segment, words, out int day, out int maxStars, out int maxWorldLevel))
            {
                if (warn != null)
                {
                    warn("Skipped '" + segment + "'. Write: day 100 = 1 star and world 2");
                }

                continue;
            }

            rows.Add(new Row(day, maxStars, maxWorldLevel));
        }

        return rows;
    }

    private static bool TryReadRow(string segment, bool words, out int day, out int maxStars, out int maxWorldLevel)
    {
        day = 0;
        maxStars = 0;
        maxWorldLevel = 0;
        if (!words)
        {
            string[] parts = segment.Split(':');
            return parts.Length == 3
                && int.TryParse(parts[0].Trim(), out day)
                && int.TryParse(parts[1].Trim(), out maxStars)
                && int.TryParse(parts[2].Trim(), out maxWorldLevel)
                && day >= 0
                && maxStars >= 0
                && maxWorldLevel >= 0;
        }

        var numbers = new List<int>();
        int i = 0;
        while (i < segment.Length)
        {
            if (segment[i] == '-' && i + 1 < segment.Length && char.IsDigit(segment[i + 1]))
            {
                return false;
            }

            if (char.IsDigit(segment[i]))
            {
                int start = i;
                while (i < segment.Length && char.IsDigit(segment[i]))
                {
                    i++;
                }

                if (!int.TryParse(segment.Substring(start, i - start), out int number))
                {
                    return false;
                }

                numbers.Add(number);
                continue;
            }

            i++;
        }

        if (numbers.Count != 3)
        {
            return false;
        }

        day = numbers[0];
        maxStars = numbers[1];
        maxWorldLevel = numbers[2];
        return true;
    }

    public static bool TryActive(IReadOnlyList<Row> rows, int day, out int maxStars, out int maxWorldLevel)
    {
        maxStars = 0;
        maxWorldLevel = 0;
        Row best = null;
        if (rows != null)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                Row row = rows[i];
                if (row.Day <= day && (best == null || row.Day >= best.Day))
                {
                    best = row;
                }
            }
        }

        if (best == null)
        {
            return false;
        }

        maxStars = best.MaxStars;
        maxWorldLevel = best.MaxWorldLevel;
        return true;
    }

    public static int CappedCreatureLevel(int rolledLevel, int maxStars)
    {
        if (rolledLevel < 1)
        {
            return 1;
        }

        if (maxStars < 0)
        {
            maxStars = 0;
        }

        int capLevel = 1 + maxStars;
        if (rolledLevel < capLevel)
        {
            return rolledLevel;
        }

        return capLevel;
    }

    public static int StarsForDay(int day, int twoStarsAfterDay, int maxStars)
    {
        if (maxStars < 0)
        {
            maxStars = 0;
        }

        if (twoStarsAfterDay > 0 && day < twoStarsAfterDay && maxStars > 1)
        {
            return 1;
        }

        return maxStars;
    }

    public static int EffectiveWorldLevel(int actualWorldLevel, int maxWorldLevel)
    {
        if (actualWorldLevel < 0)
        {
            actualWorldLevel = 0;
        }

        if (maxWorldLevel < 0)
        {
            maxWorldLevel = 0;
        }

        if (actualWorldLevel < maxWorldLevel)
        {
            return actualWorldLevel;
        }

        return maxWorldLevel;
    }

    public static float HealthBase(float creatureBaseHealth, int worldLevel, float hpMultiplier)
    {
        if (worldLevel <= 0)
        {
            return creatureBaseHealth;
        }

        return creatureBaseHealth * worldLevel * hpMultiplier;
    }

    public static float TotalDamage(float rawDamage, int worldLevel, int damagePerWorldLevel, bool creatureAttacker)
    {
        if (!creatureAttacker || worldLevel <= 0)
        {
            return rawDamage;
        }

        return rawDamage + (worldLevel * damagePerWorldLevel);
    }
}
