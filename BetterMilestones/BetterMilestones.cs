using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Text;
using System.Windows.Input;
using UnityEngine;
using UnityModManagerNet;

namespace BetterMilestones
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Debug mode")] public bool debugMode = true;
        [Draw("Smooth exit from milestone cinematic?")] public bool smoothExit = true;
        [Draw("Change the Self Sufficency milestone to include Factory?")] public bool changeSS = false;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class BetterMilestones : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new BetterMilestones(), modEntry, "BetterMilestones");
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    //HarmonyPatch for quiting the Milestone cinematic on button input
    [HarmonyPatch(typeof(MilestoneCinemetic), nameof(MilestoneCinemetic.update))]
    public class MilestoneCameraPatch
    {
        public static void Postfix(MilestoneCinemetic __instance)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //for future reference, use UnityEngine.InputLegacyModule for checking if a keybind was activated, more likey to work than InputAction
            {
                if (BetterMilestones.settings.smoothExit)
                {
                    if (BetterMilestones.settings.debugMode) Console.WriteLine("BetterMilestones - smooth transition avtivated");
                    Singleton<TimeManager>.getInstance().unpause();
                    bool tops = CoreUtils.GetMember<MilestoneCinemetic, bool>("mRenderTopsState", __instance);
                    Construction.setRenderTops(tops, force: true);
                    CoreUtils.SetMember<MilestoneCinemetic,bool>("mRestoredTops", __instance, true);
                }
                CameraManager.getInstance().setCinematic(null);
            }
        }
    }
    //HarmonyPatch that moddifies requirements for Self Sufficency milestone (Factory is pretty important)
    [HarmonyPatch(typeof(MilestoneSelfSufficiency), methodType: MethodType.Constructor)]
    public class RequirementPatch
    {
        /*public static void Postfix(Milestone __instance)
        {
            //var componentList = CoreUtils.GetMember<Milestone, List<ComponentType>>("mRequiredComponents", __instance);
            //componentList.Add(TypeList<ComponentType, ComponentTypeList>.find<SemiconductorFoundry>() as ComponentType);
            //componentList.Add(TypeList<ComponentType, ComponentTypeList>.find<SparesWorkshop>() as ComponentType);

            var moduleList = CoreUtils.GetMember<Milestone, List<ModuleType>>("mRequiredModules", __instance);
            moduleList.Add(TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>() as ModuleType);
        }*/
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (BetterMilestones.settings.changeSS)
            {
                var codes = new List<CodeInstruction>(instructions);
                var targetMethod = typeof(List<ModuleType>).GetMethod("Add", new Type[] { typeof(ModuleType) });
                object targetOperand = "call !0 class Planetbase.TypeList`2<class Planetbase.ModuleType, class Planetbase.ModuleTypeList>::find<class Planetbase.ModuleTypeBioDome>()";

                // Find the index where the required modules are being set
                int insertIndex = -1;
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Call && codes[i].operand == targetOperand)
                    {
                        insertIndex = i;
                        break;
                    }
                    else
                    {
                        if (BetterMilestones.settings.debugMode) Console.WriteLine("Couldn't find the right opcode and operand.");
                    }
                }

                if (insertIndex != -1)
                {
                    // Insert the new opcode/operand pair to add ModuleTypeFactory to the required modules list
                    codes.Insert(insertIndex + 1, new CodeInstruction(OpCodes.Ldarg_0)); // Load 'this' onto the stack
                    codes.Insert(insertIndex + 2, new CodeInstruction(OpCodes.Ldfld, typeof(MilestoneSelfSufficiency).GetField("mRequiredModules", BindingFlags.Instance | BindingFlags.NonPublic))); // Load the mRequiredModules field
                    codes.Insert(insertIndex + 3, new CodeInstruction(OpCodes.Ldsfld, typeof(TypeList<ModuleType, ModuleTypeList>).GetMethod("find").MakeGenericMethod(typeof(ModuleTypeFactory)))); // Load the ModuleTypeFactory
                    codes.Insert(insertIndex + 4, new CodeInstruction(OpCodes.Call, targetMethod)); // Call the Add method
                }
                else
                {
                    if (BetterMilestones.settings.debugMode) Console.WriteLine("IL code insertion error!");
                }

                ConstructorInfo constructor = typeof(MilestoneSelfSufficiency).GetConstructor(Type.EmptyTypes);
                if (BetterMilestones.settings.debugMode)
                {
                    OutputConstructorIL(constructor);
                }

                return codes.AsEnumerable();
            }
            return null;
        }
        public static void OutputConstructorIL(ConstructorInfo constructor)
        {
            // Get the IL code as a byte array
            byte[] ilBytes = constructor.GetMethodBody().GetILAsByteArray();

            // Convert the byte array to a readable format
            var ilReader = new ILReader(ilBytes);
            string ilCode = ilReader.ReadIL();

            // Output the IL code using Console.WriteLine
            Console.WriteLine(ilCode);
        }

        public class ILReader
        {
            private readonly byte[] _ilBytes;
            private int _position;

            public ILReader(byte[] ilBytes)
            {
                _ilBytes = ilBytes;
                _position = 0;
            }

            public string ReadIL()
            {
                var sb = new StringBuilder();
                while (_position < _ilBytes.Length)
                {
                    OpCode opCode = ReadOpCode();
                    sb.Append(opCode.ToString() + " " + opCode.OperandType.ToString() + opCode.Value.ToString());

                    // Read the operand based on the opcode type
                    switch (opCode.OperandType)
                    {
                        case OperandType.InlineNone:
                            sb.AppendLine();
                            break;
                        case OperandType.ShortInlineBrTarget:
                        case OperandType.ShortInlineI:
                        case OperandType.ShortInlineVar:
                            sb.AppendLine(" " + _ilBytes[_position++]);
                            break;
                        case OperandType.InlineVar:
                            sb.AppendLine(" " + BitConverter.ToInt16(_ilBytes, _position));
                            _position += 2;
                            break;
                        case OperandType.InlineI:
                        case OperandType.InlineBrTarget:
                        case OperandType.InlineString:
                        case OperandType.InlineSig:
                        case OperandType.InlineField:
                        case OperandType.InlineType:
                        case OperandType.InlineMethod:
                        case OperandType.InlineTok:
                            sb.AppendLine(" " + BitConverter.ToInt32(_ilBytes, _position));
                            _position += 4;
                            break;
                        case OperandType.ShortInlineR:
                            sb.AppendLine(" " + BitConverter.ToSingle(_ilBytes, _position));
                            _position += 4;
                            break;
                        case OperandType.InlineR:
                            sb.AppendLine(" " + BitConverter.ToDouble(_ilBytes, _position));
                            _position += 8;
                            break;
                        case OperandType.InlineSwitch:
                            int count = BitConverter.ToInt32(_ilBytes, _position);
                            _position += 4;
                            sb.AppendLine(" " + count);
                            for (int i = 0; i < count; i++)
                            {
                                sb.AppendLine(" " + BitConverter.ToInt32(_ilBytes, _position));
                                _position += 4;
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Unknown operand type.");
                    }
                }
                return sb.ToString();
            }

            private OpCode ReadOpCode()
            {
                byte code = _ilBytes[_position++];
                if (code != 0xFE)
                {
                    return SingleByteOpCodes[code];
                }
                else
                {
                    code = _ilBytes[_position++];
                    return MultiByteOpCodes[code];
                }
            }

            private static readonly OpCode[] SingleByteOpCodes = new OpCode[0x100];
            private static readonly OpCode[] MultiByteOpCodes = new OpCode[0x100];

            static ILReader()
            {
                foreach (FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field.FieldType == typeof(OpCode))
                    {
                        OpCode opCode = (OpCode)field.GetValue(null);
                        if (opCode.Size == 1)
                        {
                            SingleByteOpCodes[opCode.Value] = opCode;
                        }
                        else
                        {
                            MultiByteOpCodes[opCode.Value & 0xFF] = opCode;
                        }
                    }
                }
            }
        }
    }
}
