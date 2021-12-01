using Common;

"input.txt"
    .Map(PathToFile)
    .Map(ReadFile)
    .Map(NewCounter)
    .Map(SetFirstValue)
    .Tee(LoopRowsWithCalc)
    .Map(c => c.Increases)
    .Print();

string PathToFile(string filename) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

List<int> ReadFile(string filePath) => filePath.Map(File.ReadAllLines).Select(int.Parse).ToList();

Counter NewCounter(List<int> rows) => new(rows);

Counter SetFirstValue(Counter c) =>
    ReportSet(c, c.Rows.Take(3).Sum(), "N / A - no previous measurement");

void LoopRowsWithCalc(Counter c) =>
    c.Rows
        .Where((v, indx) => indx != 0 && indx < c.Rows.Count - 1)
        .Select((v, indx) => c.Rows.Skip(indx + 1).Take(3).Sum()).ToList()
        .ForEach(r => Calc(c, r));

Counter Calc(Counter c, int curr) =>
    curr == c.PrevDepth ?
        ReportSet(c, curr, "no change") :
        curr > c.PrevDepth ?
            ReportSet(c, curr, "increased").Tee(ct => ct.Increases++) :
            ReportSet(c, curr, "decreased");

Counter ReportSet(Counter c, int curr, string msg) =>
    curr.Tee(c.SetPreviousValue)
        .Map(p => $"{p}({msg})")
        .Print()
        .Map(_ => c);

public class Counter
{
    public Counter(List<int> rows)
    {
        Rows = rows;
    }

    public int Increases { get; set; }
    public int PrevDepth { get; set; }
    public List<int> Rows { get; set; }

    public void SetPreviousValue(int newVal) => PrevDepth = newVal;
}