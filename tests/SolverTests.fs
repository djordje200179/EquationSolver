module EquationSolver.SolverTests

open System
open EquationSolver
open EquationSolver.Solver
open EquationSolver.TestHelpers
open Expecto

[<Tests>]
let tests =
    testList "Solver" [
        testList "Roots" [
            testCase "Constant nonzero function has no solutions" (fun () ->
                let solutions =
                    Expect.wantOk
                        (findSolutions validConfig (fun _ -> 1.0) (interval (-2.0, 2.0)))
                        "Expected successful solve"
                Expect.isEmpty solutions "Expected no roots")

            for slope, intercept, lower, upper in [
                -1.0, 0.0, -2.0, 2.0
                2.0, -4.0, 0.0, 4.0
                -0.5, -1.5, -5.0, 0.0
            ] do
                testCase $"Linear function {slope}x + {intercept}" (fun () ->
                    let solutions =
                        Expect.wantOk
                            (findSolutions validConfig (fun x -> slope * x + intercept) (interval (lower, upper)))
                            "Expected successful solve"

                    assertSolutions [ -intercept / slope ] solutions)

            for lower, upper in [ -2.0, 2.0; -1.0, 2.0; -2.0, 1.0 ] do
                testCase $"Quadratic roots within {lower} to {upper}" (fun () ->
                    let solutions =
                        Expect.wantOk
                            (findSolutions validConfig (fun x -> x * x - 1.0) (interval (lower, upper)))
                            "Expected successful solve"
                    assertSolutions [ -1.0; 1.0 ] solutions)

            testCase "Root outside interval is not returned" (fun () ->
                let solutions =
                    Expect.wantOk
                        (findSolutions validConfig (fun x -> x - 3.0) (interval (-2.0, 2.0)))
                        "Expected successful solve"
                Expect.isEmpty solutions "Expected no roots")

            testCase "Multiple roots are returned in ascending order" (fun () ->
                let solutions =
                    Expect.wantOk
                        (findSolutions validConfig (fun x -> (x + 2.0) * x * (x - 2.0)) (interval (-3.0, 3.0)))
                        "Expected successful solve"

                assertSolutions [ -2.0; 0.0; 2.0 ] solutions)

            testCase "Closely spaced roots are found when resolution separates them" (fun () ->
                let firstRoot = 0.1
                let secondRoot = 0.105
                let solutions =
                    Expect.wantOk
                        (findSolutions validConfig (fun x -> (x - firstRoot) * (x - secondRoot)) (interval (0.0, 0.2)))
                        "Expected successful solve"

                assertSolutions [ firstRoot; secondRoot ] solutions)

            testCase "Tangent root is detected when sampled exactly" (fun () ->
                let solutions =
                    Expect.wantOk
                        (findSolutions validConfig (fun x -> x * x) (interval (-1.0, 1.0)))
                        "Expected successful solve"
                assertSolutions [ 0.0 ] solutions)
        ]

        testList "Invalid evaluations" [
            testCase "NaN samples are ignored" (fun () ->
                let solutions =
                    Expect.wantOk
                        (findSolutions validConfig (fun x -> if x < 0.0 then Double.NaN else x + 1.0) (interval (-1.0, 1.0)))
                        "Expected successful solve"

                Expect.isEmpty solutions "Expected no roots")

            testCase "Discontinuous function returns a structured solver error" (fun () ->
                let error =
                    Expect.wantError
                        (findSolutions validConfig (fun x -> 1.0 / x) (interval (-1.0, 1.0)))
                        "Expected a solver error"
                Expect.contains [ DidNotConverge; PossibleDiscontinuity ] error "Expected a convergence or discontinuity error")
        ]
    ]
