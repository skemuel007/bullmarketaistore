using System.Numerics;
using BullMarket.Domain.Common;

public sealed record ProductTechnicalDetails
{
    public ProductTechnicalDetails(
        string? cpu,
        int? cpuCores,
        int? ramGb,
        int? maxRamGb,
        string? gpu,
        string? npu,
        int? vramGb,
        int? npuTops,
        int? storageGb,
        string? storageType,
        string? display,
        decimal? batteryWh,
        decimal? weightKg)
    {
        ValidateNonNegative(cpuCores, nameof(cpuCores));
        ValidateNonNegative(ramGb, nameof(ramGb));
        ValidateNonNegative(maxRamGb, nameof(maxRamGb));
        ValidateNonNegative(vramGb, nameof(vramGb));
        ValidateNonNegative(npuTops, nameof(npuTops));
        ValidateNonNegative(storageGb, nameof(storageGb));
        ValidateNonNegative(batteryWh, nameof(batteryWh));
        ValidateNonNegative(weightKg, nameof(weightKg));

        if (ramGb is not null && maxRamGb is not null && maxRamGb < ramGb)
        {
            throw new DomainException("Max RAM cannot be less than installed RAM.", nameof(maxRamGb));
        }

        Cpu = cpu?.Trim();
        CpuCores = cpuCores;
        RamGb = ramGb;
        MaxRamGb = maxRamGb;
        Gpu = gpu?.Trim();
        Npu = npu?.Trim();
        VramGb = vramGb;
        NpuTops = npuTops;
        StorageGb = storageGb;
        StorageType = storageType?.Trim();
        Display = display?.Trim();
        BatteryWh = batteryWh;
        WeightKg = weightKg;
    }

    public string? Cpu { get; }
    public int? CpuCores { get; }
    public int? RamGb { get; }
    public int? MaxRamGb { get; }
    public string? Gpu { get; }
    public string? Npu { get; }
    public int? VramGb { get; }
    public int? NpuTops { get; }
    public int? StorageGb { get; }
    public string? StorageType { get; }
    public string? Display { get; }
    public decimal? BatteryWh { get; }
    public decimal? WeightKg { get; }

    private static void ValidateNonNegative<T>(T? value, string paramName) where T : struct, INumber<T>
    {
        if (value is not null && value.Value < T.Zero)
        {
            throw new DomainException($"{paramName} cannot be negative.", paramName);
        }
    }
}
