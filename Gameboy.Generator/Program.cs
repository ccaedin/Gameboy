﻿using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace Gameboy.Generator;

public class Program
{
    public static void Main(string[] args)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gameboy.Generator.ops.json");
        var ops = JsonDocument.Parse(stream);
        var unprefixed = ops.RootElement.GetProperty("unprefixed").EnumerateObject().Select(op =>
        {
            var opcode = Convert.ToInt32(op.Name, 16);
            var mnemonic = op.Value.GetProperty("mnemonic").GetString();
            mnemonic = mnemonic.Contains("ILLEGAL") ? "ILLEGAL" : mnemonic;
            var bytes = op.Value.GetProperty("bytes").GetInt32();
            var cycles = op.Value.GetProperty("cycles").EnumerateArray().Select(x => x.GetInt32()).ToArray();
            var operands = op.Value.GetProperty("operands").EnumerateArray().Select(o => new Operand(o.GetProperty("name").GetString()!, o.GetProperty("immediate").GetBoolean())).ToArray();
            var immediate = op.Value.GetProperty("immediate").GetBoolean();
            var jsonFlags = op.Value.GetProperty("flags");
            var flags = new InstructionFlags(jsonFlags.GetProperty("Z").GetString(), jsonFlags.GetProperty("N").GetString(), jsonFlags.GetProperty("H").GetString(), jsonFlags.GetProperty("C").GetString());
            return new Instruction(mnemonic, opcode, bytes, cycles, operands, immediate, flags);
        }).ToLookup(op => op.Mnemonic)
        .ToDictionary(g => g.Key, g => g.ToList());

        var prefixed = ops.RootElement.GetProperty("cbprefixed").EnumerateObject().Select(op =>
        {
            var opcode = Convert.ToInt32(op.Name, 16);
            var mnemonic = op.Value.GetProperty("mnemonic").GetString();
            var bytes = op.Value.GetProperty("bytes").GetInt32();
            var cycles = op.Value.GetProperty("cycles").EnumerateArray().Select(x => x.GetInt32()).ToArray();
            var operands = op.Value.GetProperty("operands").EnumerateArray().Select(o => new Operand(o.GetProperty("name").GetString()!, o.GetProperty("immediate").GetBoolean())).ToArray();
            var immediate = op.Value.GetProperty("immediate").GetBoolean();
            var jsonFlags = op.Value.GetProperty("flags");
            var flags = new InstructionFlags(jsonFlags.GetProperty("Z").GetString(), jsonFlags.GetProperty("N").GetString(), jsonFlags.GetProperty("H").GetString(), jsonFlags.GetProperty("C").GetString());
            return new Instruction(mnemonic, opcode, bytes, cycles, operands, immediate, flags);
        }).ToLookup(op => op.Mnemonic)
        .ToDictionary(g => g.Key, g => g.ToList());
    }
}