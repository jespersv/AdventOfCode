using Common;

"input.txt"
    .Map(PathToFile)
    .Map(fp => new Counter(fp))
    .Tee(ReadFile)
    .Map(SetFirstValue)
    .Tee(LoopRowsWithCalc)
    .Map(c => c.Increases)
    .Print();

Counter ReportSet(Counter c, int curr, string msg) =>
    curr.Tee(p => c.PrevDepth = p)
        .Map(p => $"{p}({msg})")
        .Print()
        .Map(p => c);

string PathToFile(string filename) =>
    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

void ReadFile(Counter counter) =>
    counter.Rows = counter.FilePath.Map(File.ReadAllLines).Select(int.Parse).ToList();

Counter SetFirstValue(Counter c) =>
    ReportSet(c, c.Rows.First(), "N / A - no previous measurement");

void LoopRowsWithCalc(Counter c) =>
    c.Rows.ForEach(r => Calc(c, r));

Counter Calc(Counter c, int curr) =>
    curr > c.PrevDepth ?
        ReportSet(c, curr, "increased").Tee(ct => ct.Increases++) :
        ReportSet(c, curr, "decreased");

public class Counter
{
    public string FilePath { get; }

    public Counter(string filePath)
    {
        FilePath = filePath;
    }

    public int Increases { get; set; }
    public int PrevDepth { get; set; }
    public List<int> Rows { get; set; }
}