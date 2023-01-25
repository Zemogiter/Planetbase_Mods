using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace PatientTraders
{
    internal class TransplierImplementation
    {
        private readonly ILGenerator generator;

        private readonly List<CodeInstruction> codes = new List<CodeInstruction>();

        private Dictionary<string, CodeInstruction> lastMatches = new Dictionary<string, CodeInstruction>();

        private string lastError;

        private bool lastUseEnd;

        private CodeMatch[] lastCodeMatches;
        //
        // Summary:
        //     The current position
        //
        // Value:
        //     The index or -1 if out of bounds
        public int Pos { get; private set; } = -1;
        //
        // Summary:
        //     Gets the number of code instructions in this matcher
        //
        // Value:
        //     The count
        public int Length => codes.Count;

        public bool IsValid
        {
            get
            {
                if (Pos >= 0)
                {
                    return Pos < Length;
                }

                return false;
            }
        }
        //
        // Summary:
        //     Checks whether the position of this CodeMatcher is outside its bounds
        //
        // Value:
        //     True if this CodeMatcher is invalid
        public bool IsInvalid
        {
            get
            {
                if (Pos >= 0)
                {
                    return Pos >= Length;
                }

                return true;
            }
        }
        private void FixStart()
        {
            Pos = Math.Max(0, Pos);
        }
        //
        // Summary:
        //     Gets a match by its name
        //
        // Parameters:
        //   name:
        //     The match name
        //
        // Returns:
        //     An instruction
        public CodeInstruction NamedMatch(string name)
        {
            return lastMatches[name];
        }

        private bool MatchSequence(int start, CodeMatch[] matches)
        {
            if (start < 0)
            {
                return false;
            }

            lastMatches = new Dictionary<string, CodeInstruction>();
            foreach (CodeMatch codeMatch in matches)
            {
                if (start >= Length || !codeMatch.Matches(codes, codes[start]))
                {
                    return false;
                }

                if (codeMatch.name != null)
                {
                    lastMatches.Add(codeMatch.name, codes[start]);
                }

                start++;
            }

            return true;
        }
        private CodeMatcher Match(CodeMatch[] matches, int direction, bool useEnd)
        {

            FixStart();
            while (IsValid)
            {
                lastUseEnd = useEnd;
                lastCodeMatches = matches;
                if (MatchSequence(Pos, matches))
                {
                    if (useEnd)
                    {
                        Pos += matches.Count() - 1;
                    }

                    break;
                }

                Pos += direction;
            }

            lastError = (IsInvalid ? ("Cannot find " + matches.Join()) : null);
            return this;
        }
        //
        // Summary:
        //     Matches forward and advances position
        //
        // Parameters:
        //   useEnd:
        //     True to set position to end of match, false to set it to the beginning of the
        //     match
        //
        //   matches:
        //     Some code matches
        //
        // Returns:
        //     The same code matcher
        public CodeMatcher MatchForward(bool useEnd, params CodeMatch[] matches)
        {
            return Match(matches, 1, useEnd);
        }
    }
}
