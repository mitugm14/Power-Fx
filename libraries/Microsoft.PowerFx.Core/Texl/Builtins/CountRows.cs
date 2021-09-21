﻿//------------------------------------------------------------------------------
// <copyright file="CountRows.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.PowerFx;
using System.Collections.Generic;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // CountRows(source:*)
    internal sealed class CountRowsFunction : FunctionWithTableInput
    {
        public override bool RequiresErrorContext => true;
        public override bool IsSelfContained => true;

        public override DelegationCapability FunctionDelegationCapability { get { return DelegationCapability.Count; } }

        public CountRowsFunction()
            : base("CountRows", TexlStrings.AboutCountRows, FunctionCategories.Table | FunctionCategories.MathAndStat, DType.Number, 0, 1, 1, DType.EmptyTable)
        { }

        public override bool SupportsPaging(CallNode callNode, TexlBinding binding) { return false; }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.CountArg1 };
        }

        public override bool IsServerDelegatable(CallNode callNode, TexlBinding binding)
        {
            Contracts.AssertValue(callNode);
            Contracts.AssertValue(binding);

            if (!CheckArgsCount(callNode, binding))
                return false;

            DelegationCapability preferredFunctionDelegationCapability;
            return TryGetValidDataSourceForDelegation(callNode, binding, out var dataSource, out preferredFunctionDelegationCapability);
        }

        // See if CountDistinct delegation is available. If true, we can make use of it on primary key as a workaround for CountRows delegation
        internal bool TryGetValidDataSourceForDelegation(CallNode callNode, TexlBinding binding, out IExternalDataSource dataSource, out DelegationCapability preferredFunctionDelegationCapability)
        {
            Contracts.AssertValue(callNode);
            Contracts.AssertValue(binding);

            preferredFunctionDelegationCapability = FunctionDelegationCapability;
            // We ensure Document is available because some tests run with a null Document.
            if ((binding.Document != null
                && binding.Document.Properties.EnabledFeatures.IsEnhancedDelegationEnabled)
                && TryGetValidDataSourceForDelegation(callNode, binding, FunctionDelegationCapability, out dataSource)
                && !ExpressionContainsView(callNode, binding))
            {
                // Check that target table is not an expanded entity (1-N/N-N relationships)
                // TASK 9966488: Enable CountRows/CountIf delegation for table relationships
                TexlNode[] args = callNode.Args.Children.VerifyValue();
                if (args.Length > 0)
                {
                    if (binding.GetType(args[0]).HasExpandInfo)
                    {
                        SuggestDelegationHint(callNode, binding);
                        return false;
                    }
                    else
                        return true;
                }
            }

            if (TryGetValidDataSourceForDelegation(callNode, binding, DelegationCapability.CountDistinct, out dataSource) && OldFeatureGates.CountDistinctDelegationSupport)
            {
                preferredFunctionDelegationCapability = DelegationCapability.CountDistinct;
                return true;
            }

            if (dataSource != null && dataSource.IsDelegatable)
                binding.ErrorContainer.EnsureError(DocumentErrorSeverity.Warning, callNode, TexlStrings.OpNotSupportedByServiceSuggestionMessage_OpNotSupportedByService, Name);

            return false;
        }

        private bool ExpressionContainsView(CallNode callNode, TexlBinding binding)
        {
            Contracts.AssertValue(callNode);
            Contracts.AssertValue(binding);

            var viewFinderVisitor = new ViewFinderVisitor(binding);
            callNode.Accept(viewFinderVisitor);

            return viewFinderVisitor.ContainsView;
        }
    }
}