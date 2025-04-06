namespace EquationSolver

open Solver
open System

module Parallel =
    let private FindDistinctLimits limits parts =
        let (lowerLimit, upperLimit) = limits
        let step = (upperLimit - lowerLimit) / double parts

        seq { lowerLimit..step..upperLimit }
        |> Seq.pairwise
        |> Array.ofSeq

    let FindSolutions config limits =
        FindDistinctLimits limits Environment.ProcessorCount
        |> Array.Parallel.map (fun limits -> (FindSolutions config) limits)
        |> Seq.concat