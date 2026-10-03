using System;
using System.Linq;
using System.Threading.Tasks;
using CSVParserWinRT.ParserEngines;
using moscow_parks.Common;
class Program {
 static int count;
 static async Task Fields(BetterParserEngine engine, string input, params string[] expected) { var actual=await engine.ExtractFields(',', '"', input); if(!actual.SequenceEqual(expected)) throw new Exception("Fields failed for ["+input+"]: ["+string.Join("|",actual)+"]"); count++; }
 static void Main() { Run().GetAwaiter().GetResult(); }
 static async Task Run() {
 var engine=new BetterParserEngine();
 await Fields(engine,"alpha,beta","alpha","beta");
 await Fields(engine,",middle,","","middle","");
 await Fields(engine,"a,,b","a","","b");
 await Fields(engine,"\"last\"","last");
 await Fields(engine,"\"\",b","","b");
 await Fields(engine,"\"a,b\",c","a,b","c");
 await Fields(engine,"\"a\"\"b\",c","a\"b","c");
 await Fields(engine,"a,b\r","a","b");
 await Fields(engine,"",new string[0]);
 await Fields(engine,"a,","a","");
 try { await engine.ExtractFields(',', '\"', "\"unfinished"); throw new Exception("Expected malformed field error"); } catch(FormatException) { count++; }
 try { await engine.ExtractFields(',', '\"', "\"a\"tail"); throw new Exception("Expected trailing text error"); } catch(FormatException) { count++; }
 var custom=await engine.ExtractFields(';','\'',"'a;b';c");
 if(!custom.SequenceEqual(new [] {"a;b","c"})) throw new Exception("custom delimiters"); count++;
 var parser=new CsvParser { RawText="name,value\nfirst,\nsecond,\"x,y\"\n" };
 var rows=(await parser.Parse()).ToList();
 if(rows.Count!=2 || rows[0]["value"]!="" || rows[1]["value"]!="x,y") throw new Exception("row dictionary integration"); count++;
 var tabParser=new CsvParser { Delimiter='\t', RawText="name\tvalue\nfirst\t\n" };
 var tabRows=(await tabParser.Parse()).ToList();
 if(tabRows.Count!=1 || tabRows[0]["value"]!="") throw new Exception("tab-separated empty import field"); count++;
 var semicolonParser=new CsvParser { Delimiter=';', RawText="name;value\nfirst;\"x;y\"\n" };
 var semicolonRows=(await semicolonParser.Parse()).ToList();
 if(semicolonRows.Count!=1 || semicolonRows[0]["value"]!="x;y") throw new Exception("semicolon-separated quoted import field"); count++;
 var tasks=Enumerable.Range(0,100).Select(async n => { char delimiter=n%2==0?',':';'; var result=await engine.ExtractFields(delimiter,'"',"a"+delimiter+"b"); if(!result.SequenceEqual(new[]{"a","b"})) throw new Exception("shared engine concurrent calls"); });
 await Task.WhenAll(tasks); count++;
 Console.WriteLine("PASS: "+count+" CSV checks including 100 concurrent calls");
 }
}
