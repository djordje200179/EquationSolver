namespace EquationSolver

open System
open FsToolkit.ErrorHandling

module Parallel =
    let findSolutions config functionToSolve interval =
        interval
        |> Interval.partition Environment.ProcessorCount
        |> Array.Parallel.map (Solver.findSolutions config functionToSolve)
        |> Array.toList
        |> List.sequenceResultM
        |> Result.map (List.concat >> List.distinct)
