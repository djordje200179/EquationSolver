module EquationSolver.SolverTests

open System
open EquationSolver.Solver
open EquationSolver.TestHelpers
open Xunit

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

[<Fact>]
let ``discontinuous function returns a structured solver error`` () =
    let result =
        findSolutions
            config
            (fun x -> 1.0 / x)
            (interval (-1.0, 1.0))

    match result with
    | Error DidNotConverge
    | Error PossibleDiscontinuity -> ()
    | Ok values -> failwithf "Expected a solver error, but got %A." values
