module EquationSolver.DomainTests

open System
open EquationSolver
open EquationSolver.TestHelpers
open Expecto

[<Tests>]
let tests =
    testList "Domain" [
        testList "Invalid accuracy" [
            for value in [
                0.0
                -1.0
                Double.NaN
                Double.PositiveInfinity
                Double.NegativeInfinity
            ] do
                testCase (string value) (fun () ->
                    let error = Expect.wantError (Config.create value validResolution) "Expected invalid accuracy"
                    let actual = Expect.wantCase error <@ InvalidAccuracy @> "Expected InvalidAccuracy"
                    Expect.isTrue (actual.Equals value) "Expected original invalid accuracy")
        ]

        testList "Invalid resolution" [
            for value in [
                0.0
                -1.0
                Double.NaN
                Double.PositiveInfinity
                Double.NegativeInfinity
            ] do
                testCase (string value) (fun () ->
                    let error = Expect.wantError (Config.create validAccuracy value) "Expected invalid resolution"
                    let actual = Expect.wantCase error <@ InvalidResolution @> "Expected InvalidResolution"
                    Expect.isTrue (actual.Equals value) "Expected original invalid resolution")
        ]

        testList "Nonfinite limits" [
            for lower, upper in [
                Double.NaN, 1.0
                0.0, Double.NaN
                Double.NegativeInfinity, 1.0
                0.0, Double.PositiveInfinity
            ] do
                testCase $"{lower} to {upper}" (fun () ->
                    let error = Expect.wantError (Interval.create lower upper) "Expected nonfinite limit"
                    let value = Expect.wantCase error <@ NonFiniteLimit @> "Expected NonFiniteLimit"
                    Expect.isTrue (Double.IsNaN value || Double.IsInfinity value) "Expected nonfinite value")
        ]

        testList "Invalid order" [
            for lower, upper in [ 1.0, 1.0; 2.0, 1.0 ] do
                testCase $"{lower} to {upper}" (fun () ->
                    Expect.equal
                        (Interval.create lower upper)
                        (Error(InvalidOrder(lower, upper)))
                        "Expected increasing limits")
        ]
    ]
