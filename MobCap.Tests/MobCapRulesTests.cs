using System.Collections.Generic;
using Xunit;

public class MobCapRulesTests
{
    private static List<MobCapRules.Row> Sample()
    {
        return MobCapRules.Parse(
            "day 0 = 0 stars and world 0; day 100 = 1 star and world 2; day 300 = 2 stars and world 4",
            null);
    }

    [Fact]
    public void Parse_reads_day_stars_and_world_level()
    {
        List<MobCapRules.Row> rows = Sample();

        Assert.Equal(3, rows.Count);
        Assert.Equal(0, rows[0].Day);
        Assert.Equal(0, rows[0].MaxStars);
        Assert.Equal(0, rows[0].MaxWorldLevel);
        Assert.Equal(300, rows[2].Day);
        Assert.Equal(2, rows[2].MaxStars);
        Assert.Equal(4, rows[2].MaxWorldLevel);
    }

    [Fact]
    public void Active_row_is_latest_day_at_or_before_today()
    {
        List<MobCapRules.Row> rows = Sample();

        Assert.True(MobCapRules.TryActive(rows, 0, out int stars, out int world));
        Assert.Equal(0, stars);
        Assert.Equal(0, world);

        Assert.True(MobCapRules.TryActive(rows, 99, out stars, out world));
        Assert.Equal(0, stars);
        Assert.Equal(0, world);

        Assert.True(MobCapRules.TryActive(rows, 100, out stars, out world));
        Assert.Equal(1, stars);
        Assert.Equal(2, world);

        Assert.True(MobCapRules.TryActive(rows, 300, out stars, out world));
        Assert.Equal(2, stars);
        Assert.Equal(4, world);

        Assert.True(MobCapRules.TryActive(rows, 1000, out stars, out world));
        Assert.Equal(2, stars);
        Assert.Equal(4, world);
    }

    [Fact]
    public void Old_colon_format_still_parses()
    {
        List<MobCapRules.Row> rows = MobCapRules.Parse("0:0:0, 100:1:2, 300:2:4", null);

        Assert.Equal(3, rows.Count);
        Assert.Equal(100, rows[1].Day);
        Assert.Equal(1, rows[1].MaxStars);
        Assert.Equal(2, rows[1].MaxWorldLevel);
        Assert.True(MobCapRules.IsLegacy("0:0:0, 100:1:2, 300:2:4"));
        Assert.False(MobCapRules.IsLegacy("day 0 = 0 stars and world 0"));
    }

    [Fact]
    public void Format_writes_a_readable_list()
    {
        string text = MobCapRules.Format(Sample());

        Assert.Equal(
            "day 0 = 0 stars and world 0; day 100 = 1 star and world 2; day 300 = 2 stars and world 4",
            text);
    }

    [Fact]
    public void Day_before_first_row_is_inactive()
    {
        List<MobCapRules.Row> rows = MobCapRules.Parse("50:1:1", null);

        Assert.False(MobCapRules.TryActive(rows, 49, out _, out _));
        Assert.True(MobCapRules.TryActive(rows, 50, out int stars, out int world));
        Assert.Equal(1, stars);
        Assert.Equal(1, world);
    }

    [Fact]
    public void Empty_list_is_inactive()
    {
        Assert.False(MobCapRules.TryActive(MobCapRules.Parse("", null), 10, out _, out _));
        Assert.False(MobCapRules.TryActive(MobCapRules.Parse(null, null), 10, out _, out _));
    }

    [Fact]
    public void Bad_segments_are_skipped()
    {
        var warnings = new List<string>();
        List<MobCapRules.Row> rows = MobCapRules.Parse(
            "nope, -1:1:1, 10:1, 10:-1:2, 10:2:-1, 20:1:3",
            warnings.Add);

        Assert.Single(rows);
        Assert.Equal(20, rows[0].Day);
        Assert.Equal(1, rows[0].MaxStars);
        Assert.Equal(3, rows[0].MaxWorldLevel);
        Assert.NotEmpty(warnings);
    }

    [Fact]
    public void Duplicate_day_last_row_wins()
    {
        List<MobCapRules.Row> rows = MobCapRules.Parse("10:1:1, 10:2:5", null);

        Assert.True(MobCapRules.TryActive(rows, 10, out int stars, out int world));
        Assert.Equal(2, stars);
        Assert.Equal(5, world);
    }

    [Fact]
    public void Star_cap_is_a_ceiling()
    {
        Assert.Equal(1, MobCapRules.CappedCreatureLevel(3, 0));
        Assert.Equal(2, MobCapRules.CappedCreatureLevel(3, 1));
        Assert.Equal(3, MobCapRules.CappedCreatureLevel(5, 2));
        Assert.Equal(2, MobCapRules.CappedCreatureLevel(2, 5));
        Assert.Equal(1, MobCapRules.CappedCreatureLevel(1, 0));
    }

    [Fact]
    public void Two_stars_wait_until_the_set_day()
    {
        Assert.Equal(1, MobCapRules.StarsForDay(0, 100, 2));
        Assert.Equal(1, MobCapRules.StarsForDay(99, 100, 2));
        Assert.Equal(2, MobCapRules.StarsForDay(100, 100, 2));
        Assert.Equal(2, MobCapRules.StarsForDay(50, 0, 2));
    }

    [Fact]
    public void World_level_cap_is_a_ceiling()
    {
        Assert.Equal(2, MobCapRules.EffectiveWorldLevel(5, 2));
        Assert.Equal(1, MobCapRules.EffectiveWorldLevel(1, 4));
        Assert.Equal(0, MobCapRules.EffectiveWorldLevel(3, 0));
        Assert.Equal(0, MobCapRules.EffectiveWorldLevel(-1, 2));
    }

    [Fact]
    public void Health_uses_capped_world_level()
    {
        Assert.Equal(100f, MobCapRules.HealthBase(100f, 0, 0.5f));
        Assert.Equal(250f, MobCapRules.HealthBase(100f, 5, 0.5f));

        int level = MobCapRules.EffectiveWorldLevel(5, 2);
        Assert.Equal(100f, MobCapRules.HealthBase(100f, level, 0.5f));
    }

    [Fact]
    public void Damage_bonus_uses_capped_world_level_for_creatures_only()
    {
        Assert.Equal(70f, MobCapRules.TotalDamage(30f, 4, 10, creatureAttacker: true));

        int level = MobCapRules.EffectiveWorldLevel(4, 1);
        Assert.Equal(40f, MobCapRules.TotalDamage(30f, level, 10, creatureAttacker: true));
        Assert.Equal(30f, MobCapRules.TotalDamage(30f, 4, 10, creatureAttacker: false));
        Assert.Equal(30f, MobCapRules.TotalDamage(30f, 0, 10, creatureAttacker: true));
    }
}
