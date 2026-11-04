module EquationSolver.SolverTests

open System
open EquationSolver.Solver
open Xunit

let private accuracy = 1e-10
let private resolution = 1e-3

let private config functionUnderTest =
    {
        Function = functionUnderTest
        Accuracy = accuracy
        Resolution = resolution
    }

let private solve functionUnderTest limits =
    limits
    |> findSolutions (config functionUnderTest)
    |> Result.map Seq.toArray

let private solveInParallel functionUnderTest limits =
    limits
    |> EquationSolver.Parallel.findSolutions (config functionUnderTest)
    |> Result.map Seq.toArray

let private expectOk result =
    match result with
    | Ok values -> values
    | Error error -> failwithf "Expected Ok, but got Error %A." error

let private assertSolutions (expected: double array) (actual: double array) =
    Assert.Equal(Array.length expected, Array.length actual)

    Array.iter2
        (fun (expectedSolution: double) (actualSolution: double) ->
            Assert.Equal(expectedSolution, actualSolution, 9))
        expected
        actual

[<Fact>]
let ``constant nonzero function has no solutions`` () =
    let solutions = solve (fun _ -> 1.0) (-2.0, 2.0) |> expectOk

    Assert.Empty(solutions)

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
    let solutions = solve (fun x -> slope * x + intercept) (lowerLimit, upperLimit) |> expectOk

    assertSolutions [| expected |] solutions

[<Theory>]
[<InlineData(-2.0, 2.0)>]
[<InlineData(-1.0, 2.0)>]
[<InlineData(-2.0, 1.0)>]
let ``quadratic function finds roots on or within interval boundaries``
    lowerLimit
    upperLimit
    =
    let solutions = solve (fun x -> x * x - 1.0) (lowerLimit, upperLimit) |> expectOk

    assertSolutions [| -1.0; 1.0 |] solutions

[<Fact>]
let ``root outside interval is not returned`` () =
    let solutions = solve (fun x -> x - 3.0) (-2.0, 2.0) |> expectOk

    Assert.Empty(solutions)

[<Fact>]
let ``multiple roots are returned in ascending order`` () =
    let solutions = solve (fun x -> (x + 2.0) * x * (x - 2.0)) (-3.0, 3.0) |> expectOk

    assertSolutions [| -2.0; 0.0; 2.0 |] solutions

[<Fact>]
let ``closely spaced roots are found when resolution separates them`` () =
    let firstRoot = 0.1
    let secondRoot = 0.105
    let solutions =
        solve
            (fun x -> (x - firstRoot) * (x - secondRoot))
            (0.0, 0.2)
        |> expectOk

    assertSolutions [| firstRoot; secondRoot |] solutions

[<Fact>]
let ``tangent root is detected when sampled exactly`` () =
    let solutions = solve (fun x -> x * x) (-1.0, 1.0) |> expectOk

    assertSolutions [| 0.0 |] solutions

[<Fact>]
let ``NaN samples are ignored`` () =
    let solutions =
        solve
            (fun x -> if x < 0.0 then Double.NaN else x + 1.0)
            (-1.0, 1.0)
        |> expectOk

    Assert.Empty(solutions)

[<Theory>]
[<InlineData(0.0)>]
[<InlineData(-1.0)>]
[<InlineData(Double.NaN)>]
[<InlineData(Double.PositiveInfinity)>]
[<InlineData(Double.NegativeInfinity)>]
let ``accuracy must be a positive finite number`` invalidAccuracy =
    let invalidConfig = { config id with Accuracy = invalidAccuracy }

    let error =
        Assert.Throws<ArgumentException>(fun () ->
            findSolutions invalidConfig (-1.0, 1.0) :> obj)

    Assert.Equal("Accuracy", error.ParamName)

[<Theory>]
[<InlineData(0.0)>]
[<InlineData(-1.0)>]
[<InlineData(Double.NaN)>]
[<InlineData(Double.PositiveInfinity)>]
[<InlineData(Double.NegativeInfinity)>]
let ``resolution must be a positive finite number`` invalidResolution =
    let invalidConfig = { config id with Resolution = invalidResolution }

    let error =
        Assert.Throws<ArgumentException>(fun () ->
            findSolutions invalidConfig (-1.0, 1.0) :> obj)

    Assert.Equal("Resolution", error.ParamName)

[<Theory>]
[<InlineData(Double.NaN, 1.0)>]
[<InlineData(0.0, Double.NaN)>]
[<InlineData(Double.NegativeInfinity, 1.0)>]
[<InlineData(0.0, Double.PositiveInfinity)>]
let ``limits must be finite`` lowerLimit upperLimit =
    let error =
        Assert.Throws<ArgumentException>(fun () ->
            solve id (lowerLimit, upperLimit) :> obj)

    Assert.Equal("limits", error.ParamName)

[<Theory>]
[<InlineData(1.0, 1.0)>]
[<InlineData(2.0, 1.0)>]
let ``lower limit must be less than upper limit`` lowerLimit upperLimit =
    let error =
        Assert.Throws<ArgumentException>(fun () ->
            solve id (lowerLimit, upperLimit) :> obj)

    Assert.Equal("limits", error.ParamName)

[<Fact>]
let ``parallel solver validates limits before partitioning`` () =
    let error =
        Assert.Throws<ArgumentException>(fun () ->
            solveInParallel id (1.0, 1.0) :> obj)

    Assert.Equal("limits", error.ParamName)

[<Fact>]
let ``discontinuous function returns a structured solver error`` () =
    let result =
        findSolutions
            (config (fun x -> 1.0 / x))
            (-1.0, 1.0)

    match result with
    | Error DidNotConverge
    | Error PossibleDiscontinuity -> ()
    | Ok values -> failwithf "Expected a solver error, but got %A." values

[<Fact>]
let ``parallel solver returns the same solutions as sequential solver`` () =
    let functionUnderTest x = sin (5.0 * x)
    let limits = (-Math.PI, Math.PI)
    let sequentialSolutions = solve functionUnderTest limits |> expectOk
    let parallelSolutions = solveInParallel functionUnderTest limits |> expectOk

    assertSolutions sequentialSolutions parallelSolutions

[<Fact>]
let ``parallel solver returns partition-boundary roots once`` () =
    let solutions = solveInParallel id (-1.0, 1.0) |> expectOk

    assertSolutions [| 0.0 |] solutions
