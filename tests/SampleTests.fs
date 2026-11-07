module EquationSolver.SampleTests

open System
open EquationSolver
open FsCheck.Xunit
open Xunit

[<Theory>]
[<InlineData(0.0, 2.0, 1.0)>]
[<InlineData(-4.0, -2.0, -3.0)>]
[<InlineData(-2.0, 2.0, 0.0)>]
[<InlineData(3.0, 3.0, 3.0)>]
let ``midpoint depends on coordinates and is symmetric`` firstPoint secondPoint expected =
    let first = { Point = firstPoint; Value = Double.NaN }
    let second = { Point = secondPoint; Value = Double.PositiveInfinity }

    Assert.Equal(expected, Sample.midpoint first second)
    Assert.Equal(expected, Sample.midpoint second first)

[<Theory>]
[<InlineData(-1.0, 1.0)>]
[<InlineData(0.0, 1.0)>]
[<InlineData(0.0, -1.0)>]
[<InlineData(Double.NegativeInfinity, Double.PositiveInfinity)>]
[<InlineData(Double.PositiveInfinity, -1.0)>]
[<InlineData(Double.NegativeInfinity, 1.0)>]
[<InlineData(0.0, Double.PositiveInfinity)>]
[<InlineData(0.0, Double.NegativeInfinity)>]
let ``different signs return true`` firstValue secondValue =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    Assert.True(Sample.hasDifferentSign first second)

[<Theory>]
[<InlineData(1.0, 2.0)>]
[<InlineData(-1.0, -2.0)>]
[<InlineData(0.0, 0.0)>]
[<InlineData(-0.0, 0.0)>]
[<InlineData(Double.NaN, 1.0)>]
[<InlineData(Double.NaN, -1.0)>]
[<InlineData(Double.NaN, 0.0)>]
[<InlineData(Double.NaN, Double.NaN)>]
[<InlineData(Double.NaN, Double.PositiveInfinity)>]
[<InlineData(Double.NaN, Double.NegativeInfinity)>]
[<InlineData(Double.PositiveInfinity, 1.0)>]
[<InlineData(Double.NegativeInfinity, -1.0)>]
[<InlineData(Double.PositiveInfinity, Double.PositiveInfinity)>]
[<InlineData(Double.NegativeInfinity, Double.NegativeInfinity)>]
let ``matching signs or NaN return false`` firstValue secondValue =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    Assert.False(Sample.hasDifferentSign first second)

[<Property>]
let ``sign comparison is symmetric for generated values`` firstValue secondValue =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    let forward = Sample.hasDifferentSign first second
    let backward = Sample.hasDifferentSign second first
    forward = backward

[<Property>]
let ``sign comparison ignores generated coordinates`` firstValue secondValue firstPoint secondPoint =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    let original = Sample.hasDifferentSign first second
    let moved =
        Sample.hasDifferentSign
            { first with Point = firstPoint }
            { second with Point = secondPoint }
    original = moved
