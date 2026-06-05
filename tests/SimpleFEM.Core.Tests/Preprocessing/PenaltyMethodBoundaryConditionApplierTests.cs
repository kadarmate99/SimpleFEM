using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Preprocessing;

namespace SimpleFEM.Core.Tests.Preprocessing
{
    public class PenaltyMethodBoundaryConditionApplierTests
    {
        private static FemModel BuildHorizontalChain() =>
            new(
                new List<Node> { new(0, 0, 0), new(1, 1, 0), new(2, 2, 0) },
                new List<Material> { new(0, "m", 100) },
                new List<CrossSection> { new(0, "a", 1.0) },
                new List<ILineElement>
                {
                    new TrussElement2D(0, 0, 1, 0, 0),
                    new TrussElement2D(1, 1, 2, 0, 0),
                },
                new List<NodalLoad>(),
                new List<Support>());


        [Fact]
        public void ApplyBCs_K_RigidRestraint_UsesPenaltyStiffnessAtRightDiagonal()
        {
            const double penalty = 1e10;
            var system = new Assembler().Assemble(BuildHorizontalChain());

            int restrainedDof = system.DofMap.GlobalIndexOf(new Dof(0, DofType.Ux));
            int freeDof = system.DofMap.GlobalIndexOf(new Dof(1, DofType.Ux));
            double restrainedOriginalDiagonal = system.K[restrainedDof, restrainedDof];
            double freeOriginalDiagonal = system.K[freeDof, freeDof];

            var (k, _) = new PenaltyMethodBoundaryConditionApplier(penalty)
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Ux), Restraint.Rigid())
                ]);

            // stiffness added to the restrained diagonal
            Assert.Equal(restrainedOriginalDiagonal + penalty, k[restrainedDof, restrainedDof], penalty * 1e-6);
            Assert.Equal(freeOriginalDiagonal, k[freeDof, freeDof]);
        }

        [Fact]
        public void ApplyBCs_K_RigidRestraint_DoesNotMutateInputSystem()
        {
            const double penalty = 1e10;
            var system = new Assembler().Assemble(BuildHorizontalChain());

            int dof = system.DofMap.GlobalIndexOf(new Dof(0, DofType.Ux));
            double original = system.K[dof, dof];

            var (k, _) = new PenaltyMethodBoundaryConditionApplier(penalty)
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Ux), Restraint.Rigid())
                ]);

            Assert.Equal(original, system.K[dof, dof], Tolerances.Tol);
        }

        [Fact]
        public void ApplyBCs_K_ElasticRestraint_UsesItsOwnStiffnessAtRightDiagonal()
        {
            const double springStiffness = 5000;
            var system = new Assembler().Assemble(BuildHorizontalChain());

            int restrainedDof = system.DofMap.GlobalIndexOf(new Dof(0, DofType.Ux));
            int freeDof = system.DofMap.GlobalIndexOf(new Dof(1, DofType.Ux));
            double restrainedOriginalDiagonal = system.K[restrainedDof, restrainedDof];
            double freeOriginalDiagonal = system.K[freeDof, freeDof];

            var (k, _) = new PenaltyMethodBoundaryConditionApplier()
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Ux), Restraint.Elastic(springStiffness))
                ]);

            Assert.Equal(restrainedOriginalDiagonal + springStiffness, k[restrainedDof, restrainedDof], springStiffness * 1e-6);
            Assert.Equal(freeOriginalDiagonal, k[freeDof, freeDof]);
        }


        [Fact]
        public void ApplyBCs_K_ElasticRestraint_DoesNotMutateInputSystem()
        {
            const double springStiffness = 5000;
            var system = new Assembler().Assemble(BuildHorizontalChain());

            int dof = system.DofMap.GlobalIndexOf(new Dof(0, DofType.Ux));
            double originalDiagonal = system.K[dof, dof];

            var (k, _) = new PenaltyMethodBoundaryConditionApplier()
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Ux), Restraint.Elastic(springStiffness))
                ]);

            Assert.Equal(originalDiagonal, system.K[dof, dof], Tolerances.Tol);
        }

        [Fact]
        public void ApplyBCs_K_IsSymmetric()
        {
            const double springStiffness = 5000;

            var system = new Assembler().Assemble(BuildHorizontalChain());
            var (k, _) = new PenaltyMethodBoundaryConditionApplier()
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Ux), Restraint.Elastic(springStiffness)),
                    new RestrainedDof(new Dof(0, DofType.Uy), Restraint.Elastic(springStiffness)),
                    new RestrainedDof(new Dof(1, DofType.Ux), Restraint.Rigid()),
                    new RestrainedDof(new Dof(1, DofType.Uy), Restraint.Rigid()),
                ]);

            Assert.Equal(k.ColumnCount, k.RowCount);

            for (int i = 0; i < k.RowCount; i++)
                for (int j = 0; j < k.ColumnCount; j++)
                    Assert.Equal(k[i, j], k[j, i]);
        }


        [Fact]
        public void ApplyBCs_K_HasNonNegativeDiagonal()
        {
            const double springStiffness = 5000;

            var system = new Assembler().Assemble(BuildHorizontalChain());
            var (k, _) = new PenaltyMethodBoundaryConditionApplier()
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Ux), Restraint.Elastic(springStiffness)),
                    new RestrainedDof(new Dof(0, DofType.Uy), Restraint.Elastic(springStiffness)),
                    new RestrainedDof(new Dof(1, DofType.Ux), Restraint.Rigid()),
                    new RestrainedDof(new Dof(1, DofType.Uy), Restraint.Rigid()),
                ]);

            for (int i = 0; i < k.RowCount; i++)
                Assert.True(k[i, i] >= 0);
        }

        [Fact]
        public void ApplyBCs_K_SkipsRestraintsOnInactiveDofs()
        {
            const double springStiffness = 5000;

            var system = new Assembler().Assemble(BuildHorizontalChain());
            
            // BCs on inactive dofs 
            var (k, _) = new PenaltyMethodBoundaryConditionApplier()
                .ApplyBCs(system,
                [
                    new RestrainedDof(new Dof(0, DofType.Rz), Restraint.Elastic(springStiffness)),
                    new RestrainedDof(new Dof(1, DofType.Rz), Restraint.Rigid()),
                ]);

            // inactive dofs are not in k so nothing shuld change
            for (int i = 0; i < k.RowCount; i++)
                for (int j = 0; j < k.ColumnCount; j++)
                    Assert.Equal(system.K[i, j], k[i, j]);
        }
    }
}
