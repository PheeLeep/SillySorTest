# SillySorTest

A benchmarking program of known sorting algorithms. I made a program when I have some free time.

## Usage:

```cmd
SillySorTest [-h | --help] [-p] [--precise] [-s <(value)>] [--joke] [-j <job name>] [--start]
```

Options:
-  `-h`, `--help`       Show this message and exit.
-  `-p`                 Run test as parallel.
-  `--precise`          Use Secure RNG instead of pseudo RNG. (true random but can increase the memory usage, depending on the array size set)
-  `-s <(value)>`       Setting the size of array to use. (Default: 1024)
-  `--joke`           Allow also to use joke type sort test jobs.
-  `-j <job name>`:    Performs only one sort job, the program will output as JSON (Should be use on last parameter). To run joke-type, must include `--joke` before this parameter.
-  `--start`          Performs one or more sort test jobs. (Should be use on last parameter) If the parameter value is not set, all sort will be use (except joke type, must include `--joke`). To specify multiple jobs, separate them with comma. (e.g. --start merge,sort)

## Build
This project requires .NET 8 SDK

- On terminal, clone the repository:
```
git clone https://github.com/PheeLeep/SillySorTest.git
```
- Move to the repository's folder through `cd SillySorTest/`
- Run `dotnet restore`, then follow by `dotnet build`.

Do not use `dotnet run --project` as the program will fail due to creating its own child process for with an argument of `-j <dob name>`.
