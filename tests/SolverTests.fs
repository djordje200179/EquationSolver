module EquationSolver.SolverTests

open System
open EquationSolver
open EquationSolver.Solver
open EquationSolver.TestHelpers
open Xunit

[<Fact>]
let ``constant nonzero function has no solutions`` () =
    let solutions = solveSequentially (fun _ -> 1.0) (-2.0, 2.0) |> expectOk

    Assert.Empty(solutions)

[<Theory>]
[<InlineData(false, 0.3, 1.0)>]
[<InlineData(true, 0.3, 1.0)>]
[<InlineData(false, 0.3, 0.95)>]
[<InlineData(true, 0.3, 0.95)>]
[<InlineData(false, 2.0, 0.5)>]
[<InlineData(true, 2.0, 0.5)>]
[<InlineData(false, 2.0, 0.0)>]
[<InlineData(true, 2.0, 0.0)>]
[<InlineData(false, 2.0, 1.0)>]
[<InlineData(true, 2.0, 1.0)>]
let ``both solvers search through the upper boundary`` useParallel resolution root =
    let config =
        Config.create validAccuracy resolution
        |> Result.defaultWith (failwithf "Invalid test configuration: %A")
    let solve = if useParallel then Parallel.findSolutions else findSolutions
    let solutions =
        solve config (fun x -> x - root) (interval (0.0, 1.0))
        |> expectOk
        |> List.toArray

    assertSolutions [| root |] solutions

[<Theory>]
[<InlineData(false, 0.25)>]
[<InlineData(true, 0.25)>]
[<InlineData(false, 0.3)>]
[<InlineData(true, 0.3)>]
[<InlineData(false, 2.0)>]
[<InlineData(true, 2.0)>]
let ``both solvers sample each boundary exactly once`` useParallel resolution =
    let config =
        Config.create validAccuracy resolution
        |> Result.defaultWith (failwithf "Invalid test configuration: %A")
    let samples = ResizeArray<float>()
    let recordSample point =
        samples.Add point
        1.0
    let solve = if useParallel then Parallel.findSolutions else findSolutions

    solve config recordSample (interval (0.0, 1.0))
    |> expectOk
    |> Assert.Empty

    let expected =
        match resolution with
        | 0.25 -> [ 0.0; 0.25; 0.5; 0.75; 1.0 ]
        | 0.3 -> [ 0.0; 0.3; 0.6; 0.9; 1.0 ]
        | _ -> [ 0.0; 1.0 ]

    assertSolutions (List.toArray expected) (samples.ToArray())

[<Theory>]
[<InlineData(-1.0, 0.0, -2.0, 2.0)>]
[<InlineData(2.0, -4.0, 0.0, 4.0)>]
[<InlineData(-0.5, -1.5, -5.0, 0.0)>]
let ``linear function has its expected solution``
    slope
    intercept
    lowerLimit
    upperLimit
    =
    let expected = -intercept / slope
    let solutions =
        solveSequentially (fun x -> slope * x + intercept) (lowerLimit, upperLimit)
        |> expectOk

    assertSolutions [| expected |] solutions

[<Theory>]
[<InlineData(-2.0, 2.0)>]
[<InlineData(-1.0, 2.0)>]
[<InlineData(-2.0, 1.0)>]
let ``quadratic function finds roots on or within interval boundaries``
    lowerLimit
    upperLimit
    =
    let solutions =
        solveSequentially (fun x -> x * x - 1.0) (lowerLimit, upperLimit)
        |> expectOk

    assertSolutions [| -1.0; 1.0 |] solutions

[<Fact>]
let ``root outside interval is not returned`` () =
    let solutions = solveSequentially (fun x -> x - 3.0) (-2.0, 2.0) |> expectOk

    Assert.Empty(solutions)

[<Fact>]
let ``multiple roots are returned in ascending order`` () =
    let solutions =
        solveSequentially (fun x -> (x + 2.0) * x * (x - 2.0)) (-3.0, 3.0)
        |> expectOk

    assertSolutions [| -2.0; 0.0; 2.0 |] solutions

[<Fact>]
let ``closely spaced roots are found when resolution separates them`` () =
    let firstRoot = 0.1
    let secondRoot = 0.105
    let solutions =
        solveSequentially
            (fun x -> (x - firstRoot) * (x - secondRoot))
            (0.0, 0.2)
        |> expectOk

    assertSolutions [| firstRoot; secondRoot |] solutions

[<Fact>]
let ``tangent root is detected when sampled exactly`` () =
    let solutions = solveSequentially (fun x -> x * x) (-1.0, 1.0) |> expectOk

    assertSolutions [| 0.0 |] solutions

[<Fact>]
let ``NaN samples are ignored`` () =
    let solutions =
        solveSequentially
            (fun x -> if x < 0.0 then Double.NaN else x + 1.0)
            (-1.0, 1.0)
        |> expectOk

    Assert.Empty(solutions)

[<Fact>]
let ``discontinuous function returns a structured solver error`` () =
    let result =
        findSolutions
            validConfig
            (fun x -> 1.0 / x)
            (interval (-1.0, 1.0))

    match result with
    | Error DidNotConverge
    | Error PossibleDiscontinuity -> ()
    | Ok values -> failwithf "Expected a solver error, but got %A." values
