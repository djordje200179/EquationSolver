namespace EquationSolver

open System

type ConfigError =
    | InvalidAccuracy of double
    | InvalidResolution of double

type Config = private {
    Accuracy: double
    Resolution: double
}

module Config =
    let create accuracy resolution =
        if not (Double.IsFinite accuracy) || accuracy <= 0.0 then
            Error(InvalidAccuracy accuracy)
        elif not (Double.IsFinite resolution) || resolution <= 0.0 then
            Error(InvalidResolution resolution)
        else
            Ok {
                Accuracy = accuracy
                Resolution = resolution
            }

    let internal accuracy config = config.Accuracy
    let internal resolution config = config.Resolution

type IntervalError =
    | NonFiniteLimit of double
    | InvalidOrder of lower: double * upper: double

type Interval = private Interval of lower: double * upper: double

module Interval =
    let create lower upper =
        if not (Double.IsFinite lower) then
            Error(NonFiniteLimit lower)
        elif not (Double.IsFinite upper) then
            Error(NonFiniteLimit upper)
        elif lower >= upper then
            Error(InvalidOrder(lower, upper))
        else
            Ok(Interval(lower, upper))

    let internal points resolution (Interval(lower, upper)) =
        seq { lower..resolution..upper }
