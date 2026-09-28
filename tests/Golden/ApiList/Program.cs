// Lists the public API of the published RandomNameGeneratorLibrary 2.2.0 by reflection (package-modernize, Phase 0).
// One line per public type, base type, interface, constructor, method, property and field, with parameter names,
// sorted ordinally. The tests check that every line still exists. Usage: dotnet run -- <output.txt>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RandomNameGeneratorLibrary;

public static class Program
{
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    public static int Main(string[] args)
    {
        var asm = typeof(PersonNameGenerator).Assembly;
        var lines = new List<string>();
        foreach (var t in asm.GetExportedTypes())
        {
            var kind = t.IsInterface ? "interface" : t.IsEnum ? "enum" : t.IsValueType ? "struct" : t.IsAbstract && t.IsSealed ? "static class" : t.IsAbstract ? "abstract class" : t.IsSealed ? "sealed class" : "class";
            lines.Add(Name(t) + " type " + kind);
            if (t.BaseType != null && t.BaseType != typeof(object))
            {
                lines.Add(Name(t) + " base " + Name(t.BaseType));
            }

            foreach (var i in t.GetInterfaces())
            {
                lines.Add(Name(t) + " implements " + Name(i));
            }

            // Protected constructors and fields count too: a caller's subclass of a public unsealed class uses them.
            foreach (var c in t.GetConstructors(Declared | BindingFlags.NonPublic).Where(c => c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly))
            {
                lines.Add(Name(t) + " constructor " + (c.IsPublic ? "" : "protected ") + ".ctor(" + Params(c) + ")");
            }

            foreach (var m in t.GetMethods(Declared | BindingFlags.NonPublic).Where(m => (m.IsPublic || m.IsFamily || m.IsFamilyOrAssembly) && !m.IsSpecialName))
            {
                var access = m.IsPublic ? "" : "protected ";
                var stat = m.IsStatic ? "static " : "";
                var ext = m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false) ? "extension " : "";
                var generic = m.IsGenericMethodDefinition ? "<" + string.Join(", ", m.GetGenericArguments().Select(a => a.Name)) + ">" : "";
                lines.Add(Name(t) + " method " + access + stat + ext + Name(m.ReturnType) + " " + m.Name + generic + "(" + Params(m) + ")");
            }

            foreach (var p in t.GetProperties(Declared))
            {
                var acc = (p.GetGetMethod() != null ? "get; " : "") + (p.GetSetMethod() != null ? "set; " : "");
                lines.Add(Name(t) + " property " + Name(p.PropertyType) + " " + p.Name + " { " + acc + "}");
            }

            foreach (var f in t.GetFields(Declared | BindingFlags.NonPublic).Where(f => f.IsPublic || f.IsFamily || f.IsFamilyOrAssembly))
            {
                lines.Add(Name(t) + " field " + (f.IsPublic ? "" : "protected ") + (f.IsStatic ? "static " : "") + (f.IsInitOnly ? "readonly " : "") + Name(f.FieldType) + " " + f.Name);
            }
        }

        lines.Sort(StringComparer.Ordinal);
        var header = new[]
        {
            "# Public API of the published RandomNameGeneratorLibrary 2.2.0 (lib/" + (Environment.Version.Major >= 5 ? "net10.0" : "netstandard2.0") + "/RandomNameGeneratorLibrary.dll from nuget.org),",
            "# listed by reflection on " + DateTime.UtcNow.ToString("yyyy-MM-dd") + " with tests/Golden/ApiList. The tests check every line still exists and nothing was",
            "# added without a plan decision. Parameter names are listed because callers use them in named arguments.",
        };
        File.WriteAllText(args[0], string.Join("\n", header.Concat(lines)) + "\n", new UTF8Encoding(false));
        Console.Error.WriteLine("wrote " + lines.Count + " lines");
        return 0;
    }

    private static string Params(MethodBase m)
    {
        return string.Join(", ", m.GetParameters().Select(p =>
            Name(p.ParameterType) + " " + p.Name + (p.HasDefaultValue ? " = " + (p.DefaultValue ?? "null") : "")));
    }

    private static string Name(Type t)
    {
        if (t.IsGenericParameter)
        {
            return t.Name;
        }

        if (t.IsByRef)
        {
            return "ref " + Name(t.GetElementType());
        }

        if (t.IsGenericType)
        {
            var def = t.GetGenericTypeDefinition();
            var baseName = (def.Namespace == null ? "" : def.Namespace + ".") + def.Name.Substring(0, def.Name.IndexOf('`'));
            if (def == typeof(Nullable<>))
            {
                return Name(t.GetGenericArguments()[0]) + "?";
            }

            return baseName + "<" + string.Join(", ", t.GetGenericArguments().Select(Name)) + ">";
        }

        return t.FullName ?? t.Name;
    }
}
