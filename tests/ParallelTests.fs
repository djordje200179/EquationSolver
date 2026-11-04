module EquationSolver.ParallelTests

open System
open EquationSolver.TestHelpers
open Xunit

[<Fact>]
let ``parallel solver returns the same solutions as sequential solver`` () =
    let functionToSolve x = sin (5.0 * x)
    let limits = -Math.PI, Math.PI
    let sequentialSolutions = solveSequentially functionToSolve limits |> expectOk
    let parallelSolutions = solveInParallel functionToSolve limits |> expectOk

    assertSolutions sequentialSolutions parallelSolutions

[<Fact>]
let ``parallel solver returns partition-boundary roots once`` () =
    let solutions = solveInParallel id (-1.0, 1.0) |> expectOk

    assertSolutions [| 0.0 |] solutions
