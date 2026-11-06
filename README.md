# EquationSolver

An F# library for finding real roots within a finite interval using sampling
and bisection. Supports sequential and parallel solving.

## Requirements

.NET 10 SDK (F# 10).

## Usage

```fsharp
open EquationSolver

match Config.create 1e-10 1e-3, Interval.create -2.0 2.0 with
| Ok config, Ok interval ->
    match Solver.findSolutions config (fun x -> x * x - 1.0) interval with
    | Ok roots -> roots |> List.iter (printfn "%.10f")
    | Error error -> printfn "Solver error: %A" error
| Error error, _ -> printfn "Invalid configuration: %A" error
| _, Error error -> printfn "Invalid interval: %A" error
```

Use `Parallel.findSolutions` with the same arguments for parallel solving.
The supplied function must be thread-safe.

## Configuration

| Property | Description |
| --- | --- |
| `Accuracy` | Function-value tolerance, not root-position tolerance. |
| `Resolution` | Sampling step used to locate root regions. |

Both values must be positive and finite. Interval limits must be finite and
strictly increasing. Both constructors return `Result`.

## Behavior and limitations

- Returns a sorted list of roots without exact duplicates, or a solver error.
- Sampling can miss closely spaced roots; tangent roots must be sampled exactly.
- Bisection assumes continuity. `NaN` samples do not form root brackets.

## Testing

```powershell
dotnet test EquationSolver.sln
```
