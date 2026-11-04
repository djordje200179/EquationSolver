namespace EquationSolver

open System
open FsToolkit.ErrorHandling

module Parallel =
    let private partitionLimits (lowerLimit, upperLimit) partitionCount =
        let width = (upperLimit - lowerLimit) / double partitionCount

        Array.init partitionCount (fun index ->
            let partitionLower = lowerLimit + double index * width
            let partitionUpper =
                if index = partitionCount - 1 then
                    upperLimit
                else
                    partitionLower + width

            partitionLower, partitionUpper)

    let findSolutions config limits =
        Solver.validate config limits

        partitionLimits limits Environment.ProcessorCount
        |> Array.Parallel.map (Solver.findSolutions config)
        |> Array.toList
        |> List.sequenceResultM
        |> Result.map (Seq.concat >> Seq.distinct)
