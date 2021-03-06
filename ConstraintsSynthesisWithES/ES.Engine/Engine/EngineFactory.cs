﻿using ES.Engine.Benchmarks;
using ES.Engine.DistanceMeasuring;
using ES.Engine.Evaluation;
using ES.Engine.Logging;
using ES.Engine.Models;
using ES.Engine.Mutation;
using ES.Engine.MutationSupervison;
using ES.Engine.PointsGeneration;
using ES.Engine.PopulationGeneration;
using ES.Engine.PrePostProcessing;
using ES.Engine.Recombination;
using ES.Engine.Selection;

namespace ES.Engine.Engine
{
    public static class EngineFactory
    {
        public static IEngine GetEngine(ExperimentParameters experimentParameters)
        {
            IEngine engine;

            //BasePopulation
            Solution[] basePopulation = new Solution[experimentParameters.BasePopulationSize];
            Solution[] offspringPopulation = new Solution[experimentParameters.OffspringPopulationSize];
            IBenchmark benchmark = BenchmarkFactory.GetBenchmark(experimentParameters);
            IPopulationGenerator populationGenerator = PopulationGeneratorsFactory.GetPopulationGenerator(experimentParameters);

            //Points generators
            //var domain = new Domain2(experimentParameters);
            //IPointsGenerator positivePointsGenerator = new PositivePointsGenerator();
            //var positivePoints =
            //    positivePointsGenerator.GeneratePoints(experimentParameters.NumberOfPositivePoints, benchmark);
            //IPointsGenerator negativePointsGenerator = new NegativePointsGenerator(positivePoints, new CanberraDistanceCalculator());

            ////Evaluator
            //var negativePoints =
            //    negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, benchmark);
            //IEvaluator evaluator = new Evaluator(experimentParameters, positivePoints, negativePoints);
            IEvaluator evaluator = new Evaluator(experimentParameters);

            //Redundant constraints remover
            var redundantConstraintsRemover = new RedundantConstraintsRemover(new DomainSpaceSampler(), benchmark, experimentParameters);

            //Statistics
            var statistics = new Statistics();

            //Logger
            ILogger logger = null;

            //Selectors
            var parentsSelector = SelectorsFactory.GetParentsSelector(experimentParameters);
            var survivorsSelector = SelectorsFactory.GetSurvivorsSelector(experimentParameters);

            //Mutation
            var objectMutator = MutatorsFactory.GetObjectMutator(experimentParameters);
            var stdDeviationsMutator = MutatorsFactory.GetStdDevsMutator(experimentParameters);
            var mutationRuleSupervisor = MutationSupervisorsFactory.GetMutationRuleSupervisor(experimentParameters);

            if (experimentParameters.TypeOfMutation == ExperimentParameters.MutationType.Correlated)
            {
                if (experimentParameters.UseRecombination)
                {
                    var objectRecombiner = RecombinersFactory.GetObjectRecombiner(experimentParameters);
                    var stdDevsRecombiner = RecombinersFactory.GetStdDevsRecombiner(experimentParameters);
                    var rotationsRecombiner = RecombinersFactory.GetRotationsRecombiner(experimentParameters);
                    var rotationsMutator = MutatorsFactory.GetRotationsMutator(experimentParameters);

                    //engine = new CmEngineWithRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, positivePointsGenerator, negativePointsGenerator, experimentParameters, basePopulation, offspringPopulation, objectRecombiner, stdDevsRecombiner, rotationsMutator, rotationsRecombiner);
                    engine = new CmEngineWithRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, redundantConstraintsRemover, experimentParameters, statistics, basePopulation, offspringPopulation, objectRecombiner, stdDevsRecombiner, rotationsMutator, rotationsRecombiner);
                }
                else
                {
                    var rotationsMutator = MutatorsFactory.GetRotationsMutator(experimentParameters);

                    //engine = new CmEngineWithoutRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, positivePointsGenerator, negativePointsGenerator, experimentParameters, basePopulation, offspringPopulation, rotationsMutator);
                    engine = new CmEngineWithoutRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, redundantConstraintsRemover, experimentParameters, statistics, basePopulation, offspringPopulation, rotationsMutator);
                }
            }
            else
            {
                if (experimentParameters.UseRecombination)
                {
                    var objectRecombiner = RecombinersFactory.GetObjectRecombiner(experimentParameters);
                    var stdDevsRecombiner = RecombinersFactory.GetStdDevsRecombiner(experimentParameters);

                    //engine = new UmEngineWithRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, positivePointsGenerator, negativePointsGenerator, experimentParameters, basePopulation, offspringPopulation, objectRecombiner, stdDevsRecombiner);
                    engine = new UmEngineWithRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, redundantConstraintsRemover, experimentParameters, statistics, basePopulation, offspringPopulation, objectRecombiner, stdDevsRecombiner);
                }
                else
                {
                    //engine = new UmEngineWithoutRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, positivePointsGenerator, negativePointsGenerator, experimentParameters, basePopulation, offspringPopulation);
                    engine = new UmEngineWithoutRecombination(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, redundantConstraintsRemover, experimentParameters, statistics, basePopulation, offspringPopulation);
                }
            }             
            
            return engine;
        }
    }
}
