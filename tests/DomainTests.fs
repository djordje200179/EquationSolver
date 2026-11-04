module EquationSolver.DomainTests

open System
open EquationSolver
open EquationSolver.TestHelpers
open Xunit

[<Theory>]
[<InlineData(0.0)>]
[<InlineData(-1.0)>]
[<InlineData(Double.NaN)>]
[<InlineData(Double.PositiveInfinity)>]
[<InlineData(Double.NegativeInfinity)>]
let ``accuracy must be a positive finite number`` invalidAccuracy =
    match Config.create invalidAccuracy resolution with
    | Error(InvalidAccuracy value) when
        value = invalidAccuracy || Double.IsNaN value && Double.IsNaN invalidAccuracy ->
        ()
    | result -> failwithf "Expected InvalidAccuracy, but got %A." result

[<Theory>]
[<InlineData(0.0)>]
[<InlineData(-1.0)>]
[<InlineData(Double.NaN)>]
[<InlineData(Double.PositiveInfinity)>]
[<InlineData(Double.NegativeInfinity)>]
let ``resolution must be a positive finite number`` invalidResolution =
    match Config.create accuracy invalidResolution with
    | Error(InvalidResolution value) when
        value = invalidResolution || Double.IsNaN value && Double.IsNaN invalidResolution ->
        ()
    | result -> failwithf "Expected InvalidResolution, but got %A." result

[<Theory>]
[<InlineData(Double.NaN, 1.0)>]
[<InlineData(0.0, Double.NaN)>]
[<InlineData(Double.NegativeInfinity, 1.0)>]
[<InlineData(0.0, Double.PositiveInfinity)>]
let ``limits must be finite`` lowerLimit upperLimit =
    match Interval.create lowerLimit upperLimit with
    | Error(NonFiniteLimit value) when
        Double.IsNaN value
        || Double.IsInfinity value ->
        ()
    | result -> failwithf "Expected NonFiniteLimit, but got %A." result

[<Theory>]
[<InlineData(1.0, 1.0)>]
[<InlineData(2.0, 1.0)>]
let ``lower limit must be less than upper limit`` lowerLimit upperLimit =
    Assert.Equal(
        Error(InvalidOrder(lowerLimit, upperLimit)),
        Interval.create lowerLimit upperLimit
    )
