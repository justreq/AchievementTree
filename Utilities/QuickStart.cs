using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

// ANDRE STINKS

namespace AchievementTree.Utilities;

// bootstrapping beacuse sandboxing takes a lot 
internal class QuickStart
{
    public static void Main(string[] args)
    {
        string file = args?.FirstOrDefault();
        Console.WriteLine(file);
        if (!File.Exists(file))
        {
            Console.WriteLine($"Missing tml path, does not exist or is not accessible");
            return;
        }
        Environment.CurrentDirectory = new FileInfo(file).Directory.FullName;

        DoRun(args);
    }
    private static List<Hook> detours = new(4);
    private static Task applyingDetoursTask;
    static void ApplyDetours()
    {
        const BindingFlags fstatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        const BindingFlags finstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        Assembly tmlAssembly = typeof(ModLoader).Assembly;

        Type amt = tmlAssembly.GetType("Terraria.ModLoader.Core.AssemblyManager");
        Type tpt = tmlAssembly.GetType("Terraria.Program");
        Type tmt = typeof(Terraria.Main);
        detours.Add(new Hook(amt.GetMethod("IsLoadable", fstatic), (Func<object, Type, bool> orig, object mod, Type type) => true, true));
        detours.Add(new Hook(amt.GetMethod("JITAssemblies", fstatic), (Action<IEnumerable<Assembly>, PreJITFilter> orig, IEnumerable<Assembly> assemblies, PreJITFilter filter) => { }, true));
        detours.Add(new Hook(tpt.GetMethod("ForceJITOnAssembly", fstatic)!, (Action<IEnumerable<Type>> orig, IEnumerable<Type> assemblies) => { }, applyByDefault: true));
        detours.Add(new Hook(tpt.GetMethod("ForceStaticInitializers", fstatic, new Type[] { typeof(Assembly) })!, (Action<Assembly> orig, Assembly assemblies) => { }, applyByDefault: true));
        detours.Add(new Hook(tmt.GetMethod("LoadContent", finstance)!, static (Action<Main> orig, Main self) =>
        {
            if (applyingDetoursTask?.IsCompleted is false)
            {
                Console.WriteLine("Waiting detours");
                applyingDetoursTask.Wait();
            }
            orig(self);
        }, true));
        detours.Add(new Hook(tmt.GetMethod("DrawSplash", finstance)!, static (Action<Main, GameTime> orig, Main self, GameTime gameTime) =>
        {
            Console.WriteLine("Fast splash start");
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 900 && Terraria.Main.showSplash; i++)
            {
                orig(self, gameTime);
                Terraria.Main.Assets.TransferCompletedAssets();
            }
            sw.Stop();
            Console.WriteLine($"Fast DrawSplash time: {sw.Elapsed}");
        }, true));
        // to trigger recompilation
        detours.Add(new Hook(amt.GetMethod("GetLoadableTypes", fstatic, new Type[] { amt.GetNestedType("ModLoadContext", fstatic | finstance), typeof(MetadataLoadContext) }),
            (Func<object, MetadataLoadContext, IDictionary<Assembly, Type[]>> orig, object mod, MetadataLoadContext mlc) => { return orig(mod, mlc); }, true));
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void DoRun(string[] args)
    {
        applyingDetoursTask = Task.Run(ApplyDetours).ContinueWith(t => Console.WriteLine("Finished applying detours"));
        //applyingDetoursTask.ContinueWith(t => { detours.ForEach(detour => detour.Dispose()); applyingDetoursTask = null; });

        string[] mainArgs = args; // new string[] { "-console" }.Concat(args).ToArray(); 
        typeof(ModLoader).Assembly.EntryPoint.Invoke(null, new object[] { mainArgs });
    }
}
