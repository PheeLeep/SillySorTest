using System;
using ILGPU;
using ILGPU.Runtime;

namespace SillySorTest.Jobs;

public class BitonicGpuSort : SortJobAbstract
{
    public override string Name => "Bitonic Sort (GPU)";

    public override string CmdName => "bitonicgpu";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        // GPU work can't be counted per element, so only comparisons are tracked.
        AccessCount = -1;
        ChangeCount = -1;

        int n = item.Count;

        // Bitonic sort needs a power-of-two length, pad with int.MaxValue (larger than any generated value).
        int paddedN = 1;
        while (paddedN < n) paddedN <<= 1;

        int[] data = new int[paddedN];
        item.CopyTo(data);
        Array.Fill(data, int.MaxValue, n, paddedN - n);

        using var context = Context.Create(builder => builder.Default());
        // Prefers CUDA, then OpenCL, then falls back to the CPU accelerator.
        using var accelerator = context.GetPreferredDevice(preferCPU: false).CreateAccelerator(context);

        var kernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<int>, int, int>(BitonicStep);

        using var buffer = accelerator.Allocate1D(data);

        for (int k = 2; k <= paddedN; k <<= 1)
        {
            for (int j = k >> 1; j > 0; j >>= 1)
            {
                kernel(paddedN, buffer.View, j, k);
                ComparisonCount += paddedN / 2;
            }
        }

        accelerator.Synchronize();
        buffer.CopyToCPU(data);

        for (int i = 0; i < n; i++)
        {
            item[i] = data[i];
        }
    }

    private static void BitonicStep(Index1D i, ArrayView<int> data, int j, int k)
    {
        int partner = i ^ j;
        if (partner <= i) return;

        bool ascending = (i & k) == 0;
        int a = data[i];
        int b = data[partner];

        if ((a > b) == ascending)
        {
            data[i] = b;
            data[partner] = a;
        }
    }
}
