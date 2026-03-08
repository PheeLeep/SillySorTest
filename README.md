# SillySorTest

A benchmarking program of known sorting algorithms. I made a program when I have some free time.

## Usage:

```cmd
SillySorTest [-h | --help] [-p] [--precise] [-s <(value)>] [--joke] [-j <job name>] [--exclude]
```

Options:
-  `-h`, `--help`       Show this message and exit.
-  `-p`                 Run test as parallel.
-  `--precise`          Use Secure RNG instead of pseudo RNG. (true random but can increase the memory usage, depending on the array size set)
-  `-s <(value)>`       Setting the size of array to use. (Default: 1024)
-  `--joke`           Allow also to use joke type sort test jobs.
-  `-j <job name>`:    Performs only one sort job, the program will output as JSON (Should be use on last parameter). To run joke-type, must include `--joke` before this parameter.
-  `--exclude`          Excludes one or more sort test jobs. To specify multiple jobs, separate them with comma. (e.g. --exclude merge,sort)

## Build
This project requires .NET 8 SDK

- On terminal, clone the repository:
```
git clone https://github.com/PheeLeep/SillySorTest.git
```
- Move to the repository's folder through `cd SillySorTest/`
- Run `dotnet restore`, then follow by `dotnet build`.

Do not use `dotnet run --project` as the program will fail due to creating its own child process for with an argument of `-j <dob name>`.

## Example

```
[i]: Test started (9 test/s [Parallel Mode])
[i]: uses Secure RNG (increases memory usage)
┌────────────┬──────────┐
│ Job        │ Status   │
├────────────┼──────────┤
│ dotnetsort │ ✅ Done  │
│ bubble     │ ✅ Done  │
│ insertion  │ ✅ Done  │
│ merge      │ ✅ Done  │
│ selection  │ ✅ Done  │
│ quicksort  │ ✅ Done  │
│ radixlsd   │ ✅ Done  │
│ radixmsd   │ ✅ Done  │
│ heap       │ ✅ Done  │
└────────────┴──────────┘
[i]: Test Completed.

Results
┌────────────┬──────────────────┬──────────────┬───────────┬──────────┬──────────┐
│ Test       │ Mem. Consumption │ Time Elapsed │ Access    │ Changes  │ Compare  │
├────────────┼──────────────────┼──────────────┼───────────┼──────────┼──────────┤
│ bubble     │ 159.54 KB        │ 00:00:07.899 │ 74951024  │ 74951024 │ 37475512 │
│ dotnetsort │ 159.54 KB        │ 00:00:00.031 │ -1        │ -1       │ 180903   │
│ heap       │ 159.54 KB        │ 00:00:00.100 │ 314190    │ 314190   │ 211215   │
│ insertion  │ 157.55 KB        │ 00:00:05.633 │ 76722667  │ 38367506 │ 38355161 │
│ merge      │ 3.5 MB           │ 00:00:00.047 │ 489969    │ 168791   │ 152387   │
│ quicksort  │ 159.54 KB        │ 00:00:00.085 │ 423653    │ 213570   │ 201866   │
│ radixlsd   │ 738.82 KB        │ 00:00:00.063 │ 444421    │ 148140   │ 0        │
│ radixmsd   │ 744.87 KB        │ 00:00:00.061 │ 444421    │ 148140   │ 0        │
│ selection  │ 159.54 KB        │ 00:00:05.909 │ 152386680 │ 37005    │ 76193340 │
└────────────┴──────────────────┴──────────────┴───────────┴──────────┴──────────┘

Memory Consumption:
  - Best: insertion (157.55 KB or 161336 bytes)
  - Worst: merge (3.5 MB or 3667040 bytes)
Time:
  - Best: dotnetsort (00:00:00.0310000)
  - Worst: bubble (00:00:07.8990000)
Least Access Count:
  - Best: heap (314190 accesses)
  - Worst: selection (152386680 accesses)
Least Change Count:
  - Best: selection (37005 changes)
  - Worst: bubble (74951024 changes)
Least Comparison Count:
  - Best: merge (152387 comparisons)
  - Worst: selection (76193340 comparisons)

System Specification:
  - CPU: AMD Ryzen 7 3750H with Radeon Vega Mobile Gfx (8 logical cores, 64-bit)
  - OS: CachyOS X64
  - .NET Version: 8.0.23
  - Total RAM: 29.3 GB (31465873408 bytes)
```