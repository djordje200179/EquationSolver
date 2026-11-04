module EquationSolver.TestHelpers

open EquationSolver
open Xunit

let accuracy = 1e-10
let resolution = 1e-3

let config =
    Config.create accuracy resolution
    |> Result.defaultWith (failwithf "Invalid test configuration: %A")

let interval (lower, upper) =
    Interval.create lower upper
    |> Result.defaultWith (failwithf "Invalid test interval: %A")

let solve functionToSolve limits =
    limits
    |> interval
    |> Solver.findSolutions config functionToSolve
    |> Result.map Seq.toArray

let solveInParallel functionToSolve limits =
    limits
    |> interval
    |> Parallel.findSolutions config functionToSolve
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
