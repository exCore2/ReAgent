using System;
using System.Linq;
using ReAgent.State;

var tests = new (string Name, Action Body)[]
{
    ("life flask maps to life buff", LifeFlask),
    ("mana flask maps to all mana buffs", ManaFlask),
    ("hybrid flask maps to life and mana buffs", HybridFlask),
    ("custom flask maps only non-empty custom buff", CustomFlask),
    ("unknown layout fails closed", UnknownLayout),
};

var failures = 0;
foreach (var (name, body) in tests)
{
    try
    {
        body();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"FAIL {name}: {ex.Message}");
    }
}

if (failures != 0)
    throw new InvalidOperationException($"{failures} ReAgent fixture test(s) failed.");

Console.WriteLine($"{tests.Length} ReAgent fixture tests passed.");

static void LifeFlask()
{
    var buffs = FlaskLayoutClassifier.GetBuffNames(1, null);
    Assert(buffs.SequenceEqual(["flask_effect_life"]), "life mapping");
}

static void ManaFlask()
{
    var buffs = FlaskLayoutClassifier.GetBuffNames(2, null);
    Assert(buffs.Count == 3 && buffs.Contains("flask_effect_mana"), "mana mapping");
}

static void HybridFlask()
{
    var buffs = FlaskLayoutClassifier.GetBuffNames(3, null);
    Assert(buffs.Count == 4 && buffs.Contains("flask_effect_life") && buffs.Contains("flask_effect_mana"), "hybrid mapping");
}

static void CustomFlask()
{
    var buffs = FlaskLayoutClassifier.GetBuffNames(4, "custom_flask_effect");
    Assert(buffs.SequenceEqual(["custom_flask_effect"]), "custom mapping");
    Assert(FlaskLayoutClassifier.GetBuffNames(4, "").Count == 0, "empty custom mapping fails closed");
}

static void UnknownLayout()
{
    Assert(FlaskLayoutClassifier.GetBuffNames(0, null).Count == 0, "unknown type fails closed");
    Assert(FlaskLayoutClassifier.GetBuffNames(99, "stale").Count == 0, "unknown type ignores stale custom data");
}

static void Assert(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}
