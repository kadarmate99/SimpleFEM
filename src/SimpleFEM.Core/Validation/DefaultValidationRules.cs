using SimpleFEM.Core.Analysis;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;
using SimpleFEM.Core.Validation.Result;
using SimpleFEM.Core.Validation.Result.Rules;

namespace SimpleFEM.Core.Validation;

internal static class DefaultValidationRules
{
    public static IEnumerable<IModelValidationRule> Model(FemAnalysisOptions o) =>
    [
        new UniqueEntityIdsRule(),
        new ReferenceIntegrityRule(),
        new ModelIsPopulatedRule(),
        new SingleAssignmentPerNodeRule(),
        new CoincidentNodesRule(o.MinNodeDistance),
        new LoadedDofsHaveStiffnessRule(),
    ];

    public static IEnumerable<IResultValidationRule> Result(FemAnalysisOptions o) =>
    [
        new EquilibriumRule(),
    ];
}