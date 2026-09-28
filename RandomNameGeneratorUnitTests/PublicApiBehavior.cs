using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    // Every line of tests/Golden/PublicApi-2.2.0.txt (listed by reflection from the published 2.2.0, tests/Golden/ApiList)
    // must still be true: type kinds, members, protected members of BaseNameGenerator and parameter names, which callers
    // use in named arguments. Package validation checks the shape against 2.2.0 but not parameter names. The listing
    // logic is a copy of tests/Golden/ApiList/Program.cs, which is frozen with the golden files.
    public class PublicApiBehavior
    {
        private const BindingFlags Declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        [Fact]
        public void Every2_2_0ApiLineStillExists()
        {
            var current = new HashSet<string>(List(typeof(PersonNameGenerator).Assembly), StringComparer.Ordinal);
            var missing = Baseline().Where(l => !current.Contains(l)).ToList();
            Assert.True(missing.Count == 0, "missing: " + string.Join(Environment.NewLine, missing));
        }

        [Fact]
        public void NoPublicMemberWasAddedWithoutAPlanDecision()
        {
            var baseline = new HashSet<string>(Baseline(), StringComparer.Ordinal);
            var added = List(typeof(PersonNameGenerator).Assembly).Where(l => !baseline.Contains(l)).ToList();
            Assert.True(added.Count == 0, "added: " + string.Join(Environment.NewLine, added));
        }

        private static IEnumerable<string> Baseline()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, "tests", "Golden", "PublicApi-2.2.0.txt");
                if (File.Exists(candidate))
                {
                    return File.ReadAllLines(candidate).Where(l => l.Length > 0 && !l.StartsWith("#", StringComparison.Ordinal)).ToList();
                }
            }

            throw new FileNotFoundException("tests/Golden/PublicApi-2.2.0.txt was not found above " + AppContext.BaseDirectory);
        }

        private static List<string> List(Assembly asm)
        {
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

            return lines;
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
                return "ref " + Name(t.GetElementType()!);
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
}
