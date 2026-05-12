using System.Text;
using CpuEmulator;

Directory.CreateDirectory("test-results");

var failures = 0;
var results = new List<string>();

try
{
    var cpu = new Cpu();
    cpu.LoadImmediate(0, 2);
    cpu.LoadImmediate(1, 3);
    cpu.Add(0, 1);

    if (cpu.Registers[0] != 5)
    {
        throw new Exception("Expected register value 5");
    }

    results.Add("<testcase name=\"Add_ShouldSumRegisters\" classname=\"CpuTests\" />");
}
catch (Exception ex)
{
    failures++;
    results.Add($"<testcase name=\"Add_ShouldSumRegisters\" classname=\"CpuTests\"><failure message=\"{ex.Message}\" /></testcase>");
}

try
{
    var cpu = new Cpu();
    cpu.LoadImmediate(0, 7);
    cpu.Store(0, 20);

    if (cpu.Memory[20] != 7)
    {
        throw new Exception("Expected memory value 7");
    }

    results.Add("<testcase name=\"Store_ShouldWriteToMemory\" classname=\"CpuTests\" />");
}
catch (Exception ex)
{
    failures++;
    results.Add($"<testcase name=\"Store_ShouldWriteToMemory\" classname=\"CpuTests\"><failure message=\"{ex.Message}\" /></testcase>");
}

var xml = new StringBuilder();
xml.AppendLine($"<testsuite tests=\"2\" failures=\"{failures}\">");
foreach (var result in results)
{
    xml.AppendLine(result);
}
xml.AppendLine("</testsuite>");

await File.WriteAllTextAsync("test-results/junit.xml", xml.ToString());

Console.WriteLine($"Tests finished. Failures: {failures}");

return failures;
