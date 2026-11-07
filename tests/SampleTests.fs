module EquationSolver.SampleTests

open System
open EquationSolver
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
let ``different signs return true regardless of order`` firstValue secondValue =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    Assert.True(Sample.hasDifferentSign first second)
    Assert.True(Sample.hasDifferentSign second first)

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
let ``matching signs or NaN return false regardless of order`` firstValue secondValue =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    Assert.False(Sample.hasDifferentSign first second)
    Assert.False(Sample.hasDifferentSign second first)

[<Theory>]
[<InlineData(-1.0, 1.0)>]
[<InlineData(1.0, 2.0)>]
let ``sign comparison is independent of point coordinates`` firstValue secondValue =
    let first = { Point = -2.0; Value = firstValue }
    let second = { Point = 3.0; Value = secondValue }

    Assert.Equal(
        Sample.hasDifferentSign first second,
        Sample.hasDifferentSign
            { first with Point = 100.0 }
            { second with Point = -100.0 }
    )
