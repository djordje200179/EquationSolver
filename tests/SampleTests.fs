module EquationSolver.SampleTests

open System
open EquationSolver
open Expecto

[<Tests>]
let tests =
    testList "Sample" [
        testList "Midpoint" [
            for firstPoint, secondPoint, expected in [
                0.0, 2.0, 1.0
                -4.0, -2.0, -3.0
                -2.0, 2.0, 0.0
                3.0, 3.0, 3.0
            ] do
                testCase (sprintf "%g and %g" firstPoint secondPoint) (fun () ->
                    let first = { Point = firstPoint; Value = Double.NaN }
                    let second = { Point = secondPoint; Value = Double.PositiveInfinity }

                    Expect.equal (Sample.midpoint first second) expected "Expected midpoint")

            testProperty "Symmetric" (fun firstPoint secondPoint ->
                let first = { Point = firstPoint; Value = 0.0 }
                let second = { Point = secondPoint; Value = 0.0 }

                let forward = Sample.midpoint first second
                let backward = Sample.midpoint second first
                forward.Equals backward)

            testProperty "Independent of values" (fun firstPoint secondPoint firstValue secondValue ->
                let first = { Point = firstPoint; Value = 0.0 }
                let second = { Point = secondPoint; Value = 0.0 }

                let original = Sample.midpoint first second
                let changed =
                    Sample.midpoint
                        { first with Value = firstValue }
                        { second with Value = secondValue }
                original.Equals changed)
        ]

        testList "Different signs" [
            for firstValue, secondValue in [
                -1.0, 1.0
                0.0, 1.0
                0.0, -1.0
                Double.NegativeInfinity, Double.PositiveInfinity
                Double.PositiveInfinity, -1.0
                Double.NegativeInfinity, 1.0
                0.0, Double.PositiveInfinity
                0.0, Double.NegativeInfinity
            ] do
                testCase (sprintf "%g and %g" firstValue secondValue) (fun () ->
                    let first = { Point = -2.0; Value = firstValue }
                    let second = { Point = 3.0; Value = secondValue }

                    Expect.isTrue (Sample.hasDifferentSign first second) "Expected different signs")
        ]

        testList "Matching signs or NaN" [
            for firstValue, secondValue in [
                1.0, 2.0
                -1.0, -2.0
                0.0, 0.0
                -0.0, 0.0
                Double.NaN, 1.0
                Double.NaN, -1.0
                Double.NaN, 0.0
                Double.NaN, Double.NaN
                Double.NaN, Double.PositiveInfinity
                Double.NaN, Double.NegativeInfinity
                Double.PositiveInfinity, 1.0
                Double.NegativeInfinity, -1.0
                Double.PositiveInfinity, Double.PositiveInfinity
                Double.NegativeInfinity, Double.NegativeInfinity
            ] do
                testCase (sprintf "%g and %g" firstValue secondValue) (fun () ->
                    let first = { Point = -2.0; Value = firstValue }
                    let second = { Point = 3.0; Value = secondValue }

                    Expect.isFalse (Sample.hasDifferentSign first second) "Expected no sign difference")
        ]

        testProperty "Sign comparison is symmetric" (fun firstValue secondValue ->
            let first = { Point = -2.0; Value = firstValue }
            let second = { Point = 3.0; Value = secondValue }

            Sample.hasDifferentSign first second = Sample.hasDifferentSign second first)

        testProperty "Sign comparison is independent of coordinates" (fun firstValue secondValue firstPoint secondPoint ->
            let first = { Point = -2.0; Value = firstValue }
            let second = { Point = 3.0; Value = secondValue }

            let original = Sample.hasDifferentSign first second
            let moved =
                Sample.hasDifferentSign
                    { first with Point = firstPoint }
                    { second with Point = secondPoint }
            original = moved)
    ]
