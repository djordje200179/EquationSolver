namespace EquationSolver

open FsToolkit.ErrorHandling

module Parallel =
    let findSolutions config functionToSolve interval =
        interval
        |> Solver.findRegions config functionToSolve
        |> Seq.toArray
        |> Array.Parallel.map (Solver.findRoot (Config.accuracy config) functionToSolve)
        |> Array.toList
        |> List.sequenceResultM
        |> Result.map List.distinct
