module EquationSolver.TestHelpers

open EquationSolver
open Xunit

let validAccuracy = 1e-10
let validResolution = 1e-3

let validConfig =
    Config.create validAccuracy validResolution
    |> Result.defaultWith (failwithf "Invalid test configuration: %A")

let interval (lower, upper) =
    Interval.create lower upper
    |> Result.defaultWith (failwithf "Invalid test interval: %A")

let solveSequentially functionToSolve limits =
    limits
    |> interval
    |> Solver.findSolutions validConfig functionToSolve
    |> Result.map Seq.toArray

let solveInParallel functionToSolve limits =
    limits
    |> interval
    |> Parallel.findSolutions validConfig functionToSolve
    |> Result.map Seq.toArray

let expectOk result =
    match result with
    | Ok value -> value
    | Error error -> failwithf "Expected Ok, but got Error %A." error

let assertSolutions (expected: double array) (actual: double array) =
    Assert.Equal(Array.length expected, Array.length actual)

    Array.iter2
        (fun (expectedSolution: double) (actualSolution: double) ->
            Assert.Equal(expectedSolution, actualSolution, 9))
        expected
        actual
