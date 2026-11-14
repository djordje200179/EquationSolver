module EquationSolver.TestHelpers

open EquationSolver
open Expecto

let validAccuracy = 1e-10
let validResolution = 1e-3

let config accuracy resolution =
    Expect.wantOk (Config.create accuracy resolution) "Expected valid test configuration"

let validConfig = config validAccuracy validResolution

let interval (lower, upper) =
    Expect.wantOk (Interval.create lower upper) "Expected valid test interval"

let assertSolutions expected actual =
    Expect.hasLength actual (List.length expected) "Expected root count"

    (expected, actual)
    ||> List.iter2 (fun expectedRoot actualRoot ->
        Expect.floatClose { absolute = 1e-9; relative = 0.0 } actualRoot expectedRoot "Expected root")
