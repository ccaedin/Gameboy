using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gameboy.Generator
{
    public enum InstructionFlagType
    {
        Untouched,
        Reset,
        Set,
        Determine,

    }
    public struct InstructionFlags
    {
        public InstructionFlagType Zero;
        public InstructionFlagType Subtract;
        public InstructionFlagType HalfCary;
        public InstructionFlagType Cary;

        public InstructionFlags(InstructionFlagType zero, InstructionFlagType subtract, InstructionFlagType halfCary, InstructionFlagType cary)
        {
            Zero = zero;
            Subtract = subtract;
            HalfCary = halfCary;
            Cary = cary;
        }

        public InstructionFlags(string z, string n, string h, string c)
        {
            var selector = (string field, string str) =>
            {
                if(field == str)
                {
                    return InstructionFlagType.Determine;
                }
                return str switch
                {
                    "-" => InstructionFlagType.Untouched,
                    "1" => InstructionFlagType.Set,
                    "0" => InstructionFlagType.Reset,
                    _ => throw new NotImplementedException(),
                };
            };
            Zero = selector("Z", z);
            Subtract = selector("N", n);
            HalfCary = selector("H", h);
            Cary = selector("C", c);
        }
    }
    public struct Operand
    {
        public readonly string Name;
        public readonly bool Immediate;
        public readonly int? Bytes;

        public Operand(string name, bool immediate, int? bytes = null)
        {
            Name = name;
            Immediate = immediate;
            Bytes = bytes;
        }
    }
    internal class Instruction
    {

        public string Mnemonic;
        public int OpCode;
        public int Bytes;
        public int[] Cycles;

        public Operand[] Operands;
        public bool Immediate;
        public InstructionFlags Flags;

        public Instruction(string mnemonic, int opCode, int bytes, int[] cycles, Operand[] operands, bool immediate, InstructionFlags flags)
        {
            Mnemonic = mnemonic;
            OpCode = opCode;
            Bytes = bytes;
            Cycles = cycles;
            Operands = operands;
            Immediate = immediate;
            Flags = flags;
        }
    }
}
