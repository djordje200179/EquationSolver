module EquationSolver.ParallelTests

open System
open System.Collections.Concurrent
open EquationSolver
open EquationSolver.TestHelpers
open Expecto

[<Tests>]
let tests =
    testList "Parallel" [
        testList "Roots" [
            testCase "Returns the same solutions as sequential solver" (fun () ->
                let functionToSolve x = sin (5.0 * x)
                let searchInterval = interval (-Math.PI, Math.PI)
                let sequential =
                    Expect.wantOk
                        (Solver.findSolutions validConfig functionToSolve searchInterval)
                        "Expected successful sequential solve"
                let concurrent =
                    Expect.wantOk
                        (Parallel.findSolutions validConfig functionToSolve searchInterval)
                        "Expected successful parallel solve"

                assertSolutions sequential concurrent)

            testCase "Returns sampled roots once" (fun () ->
                let solutions =
                    Expect.wantOk
                        (Parallel.findSolutions validConfig id (interval (-1.0, 1.0)))
                        "Expected successful parallel solve"
                assertSolutions [ 0.0 ] solutions)

            testCase "Does not introduce roots missed by the shared sampling grid" (fun () ->
                let settings = config validAccuracy 1.0
                let functionToSolve x = (x - 0.2) * (x - 0.4)
                let searchInterval = interval (0.0, 1.0)
                let sequential =
                    Expect.wantOk
                        (Solver.findSolutions settings functionToSolve searchInterval)
                        "Expected successful sequential solve"
                let concurrent =
                    Expect.wantOk
                        (Parallel.findSolutions settings functionToSolve searchInterval)
                        "Expected successful parallel solve"

                Expect.isEmpty sequential "Expected no detected roots"
                Expect.sequenceEqual concurrent sequential "Expected identical roots")

            testCase "Returns identical root approximations" (fun () ->
                let functionToSolve x = sin (5.0 * x)
                let searchInterval = interval (-Math.PI, Math.PI)
                let sequential =
                    Expect.wantOk
                        (Solver.findSolutions validConfig functionToSolve searchInterval)
                        "Expected successful sequential solve"
                let concurrent =
                    Expect.wantOk
                        (Parallel.findSolutions validConfig functionToSolve searchInterval)
                        "Expected successful parallel solve"

                Expect.sequenceEqual concurrent sequential "Expected identical approximations")

            testCase "Returns the same error as sequential solver" (fun () ->
                let functionToSolve x = 1.0 / x
                let searchInterval = interval (-1.0, 1.0)
                let sequential = Solver.findSolutions validConfig functionToSolve searchInterval
                let concurrent = Parallel.findSolutions validConfig functionToSolve searchInterval

                Expect.isError sequential "Expected a solver error"
                Expect.equal concurrent sequential "Expected identical errors")
        ]

        testList "Sampling" [
            for resolution in [ 0.125; 0.3; 2.0 ] do
                testCase (sprintf "Resolution %g uses the same points as sequential solver" resolution) (fun () ->
                    let settings = config validAccuracy resolution
                    let searchInterval = interval (0.0, 1.0)
                    let sequentialPoints = ConcurrentBag<float>()
                    let concurrentPoints = ConcurrentBag<float>()
                    let recordSample (points: ConcurrentBag<float>) point =
                        points.Add point
                        1.0

                    let sequential =
                        Expect.wantOk
                            (Solver.findSolutions settings (recordSample sequentialPoints) searchInterval)
                            "Expected successful sequential solve"
                    let concurrent =
                        Expect.wantOk
                            (Parallel.findSolutions settings (recordSample concurrentPoints) searchInterval)
                            "Expected successful parallel solve"

                    Expect.isEmpty sequential "Expected no roots"
                    Expect.isEmpty concurrent "Expected no roots"
                    Expect.sequenceEqual
                        (concurrentPoints |> Seq.sort)
                        (sequentialPoints |> Seq.sort)
                        "Expected identical sample points")
        ]
    ]
