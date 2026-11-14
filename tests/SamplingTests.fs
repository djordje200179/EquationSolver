module EquationSolver.SamplingTests

open EquationSolver
open EquationSolver.TestHelpers
open Expecto

[<Tests>]
let tests =
    testList "Sampling" [
        for solverName, solve in [
            "Sequential", Solver.findSolutions
            "Parallel", Parallel.findSolutions
        ] do
            testList solverName [
                for resolution, root in [
                    0.3, 1.0
                    0.3, 0.95
                    2.0, 0.5
                    2.0, 0.0
                    2.0, 1.0
                ] do
                    testCase (sprintf "Resolution %g finds root %g" resolution root) (fun () ->
                        let solutions =
                            Expect.wantOk
                                (solve (config validAccuracy resolution) (fun x -> x - root) (interval (0.0, 1.0)))
                                "Expected successful solve"

                        assertSolutions [ root ] solutions)

                for resolution, expected in [
                    0.25, [ 0.0; 0.25; 0.5; 0.75; 1.0 ]
                    0.3, [ 0.0; 0.3; 0.6; 0.9; 1.0 ]
                    2.0, [ 0.0; 1.0 ]
                ] do
                    testCase (sprintf "Resolution %g samples both boundaries once" resolution) (fun () ->
                        let samples = ResizeArray<float>()
                        let recordSample point =
                            samples.Add point
                            1.0

                        let solutions =
                            Expect.wantOk
                                (solve (config validAccuracy resolution) recordSample (interval (0.0, 1.0)))
                                "Expected successful solve"

                        Expect.isEmpty solutions "Expected no roots"
                        assertSolutions expected (List.ofSeq samples))
            ]
    ]
