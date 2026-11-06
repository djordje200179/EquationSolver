module EquationSolver.ParallelTests

open System
open System.Collections.Concurrent
open EquationSolver
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
let ``parallel solver returns sampled roots once`` () =
    let solutions = solveInParallel id (-1.0, 1.0) |> expectOk

    assertSolutions [| 0.0 |] solutions

[<Theory>]
[<InlineData(0.125)>]
[<InlineData(0.3)>]
[<InlineData(2.0)>]
let ``parallel solver uses the same sampling points as sequential solver`` resolution =
    let config =
        Config.create validAccuracy resolution
        |> Result.defaultWith (failwithf "Invalid test configuration: %A")
    let searchInterval = interval (0.0, 1.0)
    let sequentialPoints = ConcurrentBag<float>()
    let parallelPoints = ConcurrentBag<float>()
    let recordSample (points: ConcurrentBag<float>) point =
        points.Add point
        1.0

    Solver.findSolutions config (recordSample sequentialPoints) searchInterval
    |> expectOk
    |> Assert.Empty
    Parallel.findSolutions config (recordSample parallelPoints) searchInterval
    |> expectOk
    |> Assert.Empty

    Assert.Equal<float list>(
        sequentialPoints |> Seq.sort |> Seq.toList,
        parallelPoints |> Seq.sort |> Seq.toList
    )

[<Fact>]
let ``parallel solver does not introduce roots missed by the shared sampling grid`` () =
    let config =
        Config.create validAccuracy 1.0
        |> Result.defaultWith (failwithf "Invalid test configuration: %A")
    let functionToSolve x = (x - 0.2) * (x - 0.4)
    let searchInterval = interval (0.0, 1.0)
    let sequential = Solver.findSolutions config functionToSolve searchInterval |> expectOk
    let parallelSolutions = Parallel.findSolutions config functionToSolve searchInterval |> expectOk

    Assert.Empty sequential
    Assert.Equal<float list>(sequential, parallelSolutions)

[<Fact>]
let ``parallel solver returns identical root approximations`` () =
    let functionToSolve x = sin (5.0 * x)
    let searchInterval = interval (-Math.PI, Math.PI)
    let sequential = Solver.findSolutions validConfig functionToSolve searchInterval |> expectOk
    let parallelSolutions = Parallel.findSolutions validConfig functionToSolve searchInterval |> expectOk

    Assert.Equal<float list>(sequential, parallelSolutions)

[<Fact>]
let ``parallel solver returns the same error as sequential solver`` () =
    let functionToSolve x = 1.0 / x
    let searchInterval = interval (-1.0, 1.0)
    let sequential = Solver.findSolutions validConfig functionToSolve searchInterval
    let parallelResult = Parallel.findSolutions validConfig functionToSolve searchInterval

    Assert.True(Result.isError sequential)
    Assert.Equal<Result<float list, Solver.SolveError>>(sequential, parallelResult)
