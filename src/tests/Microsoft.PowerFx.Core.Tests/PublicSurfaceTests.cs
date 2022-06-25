﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.PowerFx.Syntax;
using Xunit;

namespace Microsoft.PowerFx.Core.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void Test()
        {
            var asm = typeof(Parser.TexlParser).Assembly;

            // The goal for public namespaces is to make the SDK easy for the consumer. 
            // Namespace principles for public classes:            // 
            // - prefer fewer namespaces. See C# for example: https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis
            // - For easy discovery, but Engine in "Microsoft.PowerFx".
            // - For sub areas with many related classes, cluster into a single subnamespace.
            // - Avoid nesting more than 1 level deep

            var allowed = new HashSet<string>()
            {
                // Core namespace. 
                "Microsoft.PowerFx.PowerFxConfig",
                "Microsoft.PowerFx.CheckResult",
                "Microsoft.PowerFx.ParseResult",
                "Microsoft.PowerFx.FunctionInfo",
                "Microsoft.PowerFx.NameCollisionException",
                "Microsoft.PowerFx.ErrorKind",
                "Microsoft.PowerFx.ExpressionError",
                "Microsoft.PowerFx.FormulaWithParameters",
                "Microsoft.PowerFx.IExpression",
                "Microsoft.PowerFx.IExpressionExtensions",
                "Microsoft.PowerFx.IPowerFxEngine",
                "Microsoft.PowerFx.ParserOptions",
                "Microsoft.PowerFx.Engine",
                "Microsoft.PowerFx.ErrorSeverity",
                "Microsoft.PowerFx.OptionSet",

                // Feature flags are experimental - hosts shouldn't use it. 
                "Microsoft.PowerFx.Preview.FeatureFlags",

                // Lexer                
                "Microsoft.PowerFx.Syntax.Span",
                "Microsoft.PowerFx.Syntax.BinaryOp",
                "Microsoft.PowerFx.Syntax.TokKind",
                "Microsoft.PowerFx.Syntax.CommentToken",
                "Microsoft.PowerFx.Syntax.ErrorToken",
                "Microsoft.PowerFx.Syntax.IdentToken",
                "Microsoft.PowerFx.Syntax.NumLitToken",
                "Microsoft.PowerFx.Syntax.StrLitToken",
                "Microsoft.PowerFx.Syntax.Token",
                "Microsoft.PowerFx.Syntax.UnaryOp",
                "Microsoft.PowerFx.Syntax.VariadicOp",

                // Parse nodes
                "Microsoft.PowerFx.Syntax.Identifier",
                "Microsoft.PowerFx.Syntax.NodeKind",
                "Microsoft.PowerFx.Syntax.AsNode",
                "Microsoft.PowerFx.Syntax.BinaryOpNode",
                "Microsoft.PowerFx.Syntax.BlankNode",
                "Microsoft.PowerFx.Syntax.BoolLitNode",
                "Microsoft.PowerFx.Syntax.CallNode",
                "Microsoft.PowerFx.Syntax.DottedNameNode",
                "Microsoft.PowerFx.Syntax.ErrorNode",
                "Microsoft.PowerFx.Syntax.FirstNameNode",
                "Microsoft.PowerFx.Syntax.ListNode",
                "Microsoft.PowerFx.Syntax.NameNode",
                "Microsoft.PowerFx.Syntax.NumLitNode",
                "Microsoft.PowerFx.Syntax.ParentNode",
                "Microsoft.PowerFx.Syntax.RecordNode",
                "Microsoft.PowerFx.Syntax.SelfNode",
                "Microsoft.PowerFx.Syntax.StrInterpNode",
                "Microsoft.PowerFx.Syntax.StrLitNode",
                "Microsoft.PowerFx.Syntax.TableNode",
                "Microsoft.PowerFx.Syntax.TexlNode",
                "Microsoft.PowerFx.Syntax.UnaryOpNode",
                "Microsoft.PowerFx.Syntax.VariadicBase",
                "Microsoft.PowerFx.Syntax.VariadicOpNode",
                              
                // Visitors
                "Microsoft.PowerFx.Syntax.IdentityTexlVisitor",
                "Microsoft.PowerFx.Syntax.TexlFunctionalVisitor`2",
                "Microsoft.PowerFx.Syntax.TexlVisitor",

                // Power Fx Type system and Values. 
                "Microsoft.PowerFx.Types.PrimitiveValueConversions",
                "Microsoft.PowerFx.Types.AggregateType",
                "Microsoft.PowerFx.Types.BlankType",
                "Microsoft.PowerFx.Types.BooleanType",
                "Microsoft.PowerFx.Types.ColorType",
                "Microsoft.PowerFx.Types.DateTimeNoTimeZoneType",
                "Microsoft.PowerFx.Types.DateTimeType",
                "Microsoft.PowerFx.Types.DateType",
                "Microsoft.PowerFx.Types.ExternalType",
                "Microsoft.PowerFx.Types.ExternalTypeKind",
                "Microsoft.PowerFx.Types.FormulaType",
                "Microsoft.PowerFx.Types.GuidType",
                "Microsoft.PowerFx.Types.HyperlinkType",
                "Microsoft.PowerFx.Types.ITypeVisitor",
                "Microsoft.PowerFx.Types.NamedFormulaType",
                "Microsoft.PowerFx.Types.NumberType",
                "Microsoft.PowerFx.Types.OptionSetValueType",
                "Microsoft.PowerFx.Types.RecordType",
                "Microsoft.PowerFx.Types.StringType",
                "Microsoft.PowerFx.Types.TableType",
                "Microsoft.PowerFx.Types.TimeType",
                "Microsoft.PowerFx.Types.BindingErrorType",
                "Microsoft.PowerFx.Types.UnknownType",
                "Microsoft.PowerFx.Types.UntypedObjectType",
                "Microsoft.PowerFx.Types.BlankValue",
                "Microsoft.PowerFx.Types.BooleanValue",
                "Microsoft.PowerFx.Types.ColorValue",
                "Microsoft.PowerFx.Types.DValue`1",
                "Microsoft.PowerFx.Types.DateTimeValue",
                "Microsoft.PowerFx.Types.DateValue",
                "Microsoft.PowerFx.Types.ErrorValue",
                "Microsoft.PowerFx.Types.FormulaValue",
                "Microsoft.PowerFx.Types.GuidValue",
                "Microsoft.PowerFx.Types.IUntypedObject",
                "Microsoft.PowerFx.Types.IValueVisitor",
                "Microsoft.PowerFx.Types.NamedValue",
                "Microsoft.PowerFx.Types.NumberValue",
                "Microsoft.PowerFx.Types.OptionSetValue",
                "Microsoft.PowerFx.Types.PrimitiveValue`1",
                "Microsoft.PowerFx.Types.RecordValue",
                "Microsoft.PowerFx.Types.StringValue",
                "Microsoft.PowerFx.Types.TableValue",
                "Microsoft.PowerFx.Types.TimeValue",
                "Microsoft.PowerFx.Types.UntypedObjectValue",
                "Microsoft.PowerFx.Types.ValidFormulaValue",
                "Microsoft.PowerFx.Types.UnsupportedType",
                "Microsoft.PowerFx.Types.CollectionTableValue`1",

                // Intellisense classes. Used primarily by the Language Service Provider.
                // Most evaluators should never need these. 
                "Microsoft.PowerFx.Intellisense.IPowerFxScope",
                "Microsoft.PowerFx.Intellisense.IIntellisenseResult",
                "Microsoft.PowerFx.Intellisense.IIntellisenseSuggestion",
                "Microsoft.PowerFx.Intellisense.IntellisenseOperations",
                "Microsoft.PowerFx.Intellisense.SignatureHelp.ParameterInformation",
                "Microsoft.PowerFx.Intellisense.SignatureHelp.SignatureHelp",
                "Microsoft.PowerFx.Intellisense.SignatureHelp.SignatureInformation",
                "Microsoft.PowerFx.Intellisense.SuggestionIconKind",
                "Microsoft.PowerFx.Intellisense.SuggestionKind",
                "Microsoft.PowerFx.Intellisense.UIString",
                "Microsoft.PowerFx.Intellisense.TokenResultType",

                // TBD ...
                "Microsoft.PowerFx.Core.RenameDriver",
                "Microsoft.PowerFx.Core.App.DefaultEnabledFeatures",
                "Microsoft.PowerFx.Core.App.IExternalEnabledFeatures",
                "Microsoft.PowerFx.Core.DisplayNameUtility",
                "Microsoft.PowerFx.Core.DisplayNameProvider",
                "Microsoft.PowerFx.Core.FormulaTypeSchema",
                "Microsoft.PowerFx.Core.FormulaTypeToSchemaConverter",
                "Microsoft.PowerFx.Core.Utils.DName",
                "Microsoft.PowerFx.Core.Utils.DPath",
                "Microsoft.PowerFx.Core.Utils.ICheckable",
                "Microsoft.PowerFx.Core.Localization.ErrorResourceKey"
            };

            var sb = new StringBuilder();
            var count = 0;
            foreach (var type in asm.GetTypes().Where(t => t.IsPublic))
            {
                var name = type.FullName;
                if (!allowed.Contains(name))
                {
                    sb.AppendLine(name);
                    count++;
                }

                allowed.Remove(name);
            }

            Assert.True(count == 0, $"Unexpected public types: {sb}");

            // Types we expect to be in the assembly are all there. 
            Assert.Empty(allowed);
        }

        // No public type with TransportType attribute
        // TransportType is special to Canvas Documents and a tool reflects over it
        // and it's very brittle.
        [Fact]
        public void NoTransportInPublicTypes()
        {
            var exceptionList = new HashSet<string>()
            {
                "Microsoft.PowerFx.Syntax.Span"
            };

            var asm = typeof(Parser.TexlParser).Assembly;
            foreach (var type in asm.GetTypes().Where(t => t.IsPublic))
            {
                if (exceptionList.Contains(type.FullName))
                {
                    continue;
                }

                var attrs = type.GetCustomAttributesData();

                var hasTransport = attrs.Any(attr => attr.AttributeType.Namespace.StartsWith("Microsoft.AppMagic.Transport"));
                Assert.False(hasTransport, $"Types '{type.FullName}' with Transport attribute shouldn't be public.");
            }
        }

        // Assert DocumentErrorSeverity and ErrorSeverity are in sync. 
        [Fact]
        public void ErrorSeverityEnumsMatch()
        {
            var values1 = Enum.GetValues(typeof(Errors.DocumentErrorSeverity));
            var values2 = Enum.GetValues(typeof(ErrorSeverity));

            var len = values1.Length;
            Assert.Equal(len, values2.Length);
            
            for (var i = 0; i < len; i++)
            {
                var x = values1.GetValue(i);
                var y = values1.GetValue(i);

                Assert.Equal((int)x, (int)y);
                Assert.Equal(x.ToString(), y.ToString());
            }
        }

        [Fact]
        public void TestTexlNodeTypes() => TestPublicClassHierarchy(typeof(TexlNode));

        [Fact]
        public void TestTokenTypes() => TestPublicClassHierarchy(typeof(Token), requireAbstractOrSealed: false);

        private static void TestPublicClassHierarchy(Type rootType, bool requireAbstractOrSealed = true)
        {
            var errors = new StringBuilder();

            var asm = rootType.Assembly;
            var types = asm.GetTypes().Where(t => IsPublicSubclassOrEqual(t, rootType)).ToList();
            Assert.True(types.Count > 0, "No types found");

            foreach (var type in types)
            {
                var fullName = type.FullName;

                // Should be abstract or sealed
                if (requireAbstractOrSealed && !(type.IsSealed || type.IsAbstract))
                {
                    errors.AppendLine($"{fullName} is neither abstract nor sealed");
                }

                // Should have immutable attribute
                if (type.GetCustomAttribute<ThreadSafeImmutableAttribute>() is null)
                {
                    errors.AppendLine($"{fullName} does not have [ThreadSafeImmutable]");
                }

                // All ctors should be internal
                foreach (var ctor in type.GetConstructors())
                {
                    if (ctor.IsPublic)
                    {
                        errors.AppendLine($"{fullName}.{ctor.Name} constructor is public");
                    }
                }

                // Should not have public fields
                foreach (var field in type.GetFields())
                {
                    if (field.IsPublic)
                    {
                        errors.AppendLine($"{fullName}.{field.Name} field is public");
                    }
                }
            }

            Assert.True(errors.Length == 0, $"TexlNode errors: {errors}");
        }

        [Fact]
        public static void TestImmutability()
        {
            var asm = typeof(Microsoft.PowerFx.Syntax.TexlNode).Assembly;
            ImmutabilityTests.CheckImmutability(asm);
        }

        /// <summary>
        ///     Checks whether <see cref="t1" /> is public, and equal to or subclass of to <see cref="t2" />.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        private static bool IsPublicSubclassOrEqual(Type t1, Type t2) => t1.IsPublic && (t1.Equals(t2) || t1.IsSubclassOf(t2));
    }
}
